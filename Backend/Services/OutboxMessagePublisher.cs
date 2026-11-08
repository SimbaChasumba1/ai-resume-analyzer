using System.Text;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace backend.Services;

public class OutboxMessagePublisher
{
    private readonly AppDbContext _db;
    private readonly ILogger<OutboxMessagePublisher> _logger;
    private readonly string _queueName;

    public OutboxMessagePublisher(
        AppDbContext db,
        ILogger<OutboxMessagePublisher> logger,
        string queueName)
    {
        _db = db;
        _logger = logger;
        _queueName = queueName;
    }

    public async Task<int> PublishPendingMessagesAsync(
        IChannel channel,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var messages = await _db.OutboxMessages
            .Where(m =>
                m.ProcessedAt == null &&
                (m.NextAttemptAt == null ||
                 m.NextAttemptAt <= now))
            .OrderBy(m => m.CreatedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        var publishedCount = 0;

        foreach (var message in messages)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var body = Encoding.UTF8.GetBytes(message.Payload);

                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json",
                    MessageId = message.Id.ToString()
                };

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: _queueName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);

            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                message.Attempts++;
                message.LastError = ex.Message;

                var delaySeconds = Math.Pow(
                    2,
                    Math.Min(message.Attempts, 6));

                message.NextAttemptAt =
                    DateTime.UtcNow.AddSeconds(delaySeconds);

                await _db.SaveChangesAsync(cancellationToken);

                _logger.LogError(
                    ex,
                    "Failed to publish outbox message {MessageId}. Attempt {Attempt}. Next retry: {NextAttemptAt}",
                    message.Id,
                    message.Attempts,
                    message.NextAttemptAt);

                continue;
            }

            message.ProcessedAt = DateTime.UtcNow;
            message.NextAttemptAt = null;
            message.LastError = null;

            await _db.SaveChangesAsync(cancellationToken);

            publishedCount++;

            _logger.LogInformation(
                "Outbox message {MessageId} published successfully for queue {QueueName}.",
                message.Id,
                _queueName);
        }

        return publishedCount;
    }
}