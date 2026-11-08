using System.Text.Json;
using backend.Models;
using RabbitMQ.Client;

namespace ResumeAnalysis.Worker;

public class WorkerMessageHandler
{
    private readonly Func<Guid, CancellationToken, Task<bool>> _processJob;
    private readonly ILogger<WorkerMessageHandler> _logger;
    private readonly TimeSpan _retryDelay;

    public WorkerMessageHandler(
        Func<Guid, CancellationToken, Task<bool>> processJob,
        ILogger<WorkerMessageHandler> logger,
        TimeSpan? retryDelay = null)
    {
        _processJob = processJob;
        _logger = logger;
        _retryDelay = retryDelay ?? TimeSpan.FromSeconds(5);
    }

    public async Task HandleAsync(
        ReadOnlyMemory<byte> body,
        ulong deliveryTag,
        IChannel channel,
        CancellationToken cancellationToken)
    {
        try
        {
            AnalysisJobMessage? message;

            try
            {
                message = JsonSerializer.Deserialize<AnalysisJobMessage>(
                    body.Span,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Received malformed analysis job message.");

                await channel.BasicRejectAsync(
                    deliveryTag,
                    requeue: false,
                    cancellationToken: cancellationToken);

                return;
            }

            if (message == null || message.JobId == Guid.Empty)
            {
                _logger.LogWarning(
                    "Received an invalid analysis job message.");

                await channel.BasicRejectAsync(
                    deliveryTag,
                    requeue: false,
                    cancellationToken: cancellationToken);

                return;
            }

            _logger.LogInformation(
                "Received analysis job {JobId}.",
                message.JobId);

            var shouldRequeue = await _processJob(
                message.JobId,
                cancellationToken);

            if (shouldRequeue)
            {
                await Task.Delay(_retryDelay, cancellationToken);

                await channel.BasicNackAsync(
                    deliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: cancellationToken);
            }
            else
            {
                await channel.BasicAckAsync(
                    deliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);
            }
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            // Host is shutting down. Leave the message unacknowledged.
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error consuming an analysis job message.");

            try
            {
                await channel.BasicNackAsync(
                    deliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: cancellationToken);
            }
            catch (Exception nackException)
            {
                _logger.LogError(
                    nackException,
                    "Could not requeue the message.");
            }
        }
    }
}