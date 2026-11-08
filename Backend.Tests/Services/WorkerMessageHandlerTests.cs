using System.Text;
using System.Text.Json;
using Moq;
using RabbitMQ.Client;
using ResumeAnalysis.Worker;
using Microsoft.Extensions.Logging;

namespace Backend.Tests.Services;

public class WorkerMessageHandlerTests
{
    private readonly Mock<IChannel> _channel = new();
    private readonly Mock<ILogger<WorkerMessageHandler>> _logger = new();

    private WorkerMessageHandler CreateHandler(
        Func<Guid, CancellationToken, Task<bool>> processJob)
    {
        return new WorkerMessageHandler(
            processJob,
            _logger.Object,
            TimeSpan.Zero);
    }

    private static ReadOnlyMemory<byte> CreateMessage(Guid jobId)
    {
        var json = JsonSerializer.Serialize(new
        {
            JobId = jobId
        });

        return Encoding.UTF8.GetBytes(json);
    }

    public WorkerMessageHandlerTests()
    {
        _channel
            .Setup(x => x.BasicAckAsync(
                It.IsAny<ulong>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        _channel
            .Setup(x => x.BasicNackAsync(
                It.IsAny<ulong>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        _channel
            .Setup(x => x.BasicRejectAsync(
                It.IsAny<ulong>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);
    }

    [Fact]
    public async Task HandleAsync_SuccessfulJob_AcknowledgesMessage()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        var processJob = new Mock<
            Func<Guid, CancellationToken, Task<bool>>>();

        processJob
            .Setup(x => x(jobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = CreateHandler(processJob.Object);

        // Act
        await handler.HandleAsync(
            CreateMessage(jobId),
            123,
            _channel.Object,
            CancellationToken.None);

        // Assert
        processJob.Verify(
            x => x(jobId, It.IsAny<CancellationToken>()),
            Times.Once);

        _channel.Verify(
            x => x.BasicAckAsync(
                123,
                false,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _channel.Verify(
            x => x.BasicNackAsync(
                It.IsAny<ulong>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_JobNeedsRetry_NacksAndRequeuesMessage()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        var handler = CreateHandler(
            (_, _) => Task.FromResult(true));

        // Act
        await handler.HandleAsync(
            CreateMessage(jobId),
            456,
            _channel.Object,
            CancellationToken.None);

        // Assert
        _channel.Verify(
            x => x.BasicNackAsync(
                456,
                false,
                true,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _channel.Verify(
            x => x.BasicAckAsync(
                It.IsAny<ulong>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_EmptyJobId_RejectsWithoutRequeue()
    {
        // Arrange
        var handler = CreateHandler(
            (_, _) => Task.FromResult(false));

        // Act
        await handler.HandleAsync(
            CreateMessage(Guid.Empty),
            789,
            _channel.Object,
            CancellationToken.None);

        // Assert
        _channel.Verify(
            x => x.BasicRejectAsync(
                789,
                false,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _channel.Verify(
            x => x.BasicAckAsync(
                It.IsAny<ulong>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_MalformedJson_RejectsWithoutRequeue()
    {
        // Arrange
        var handler = CreateHandler(
            (_, _) => Task.FromResult(false));

        var malformedMessage =
            Encoding.UTF8.GetBytes("{ invalid json }");

        // Act
        await handler.HandleAsync(
            malformedMessage,
            321,
            _channel.Object,
            CancellationToken.None);

        // Assert
        _channel.Verify(
            x => x.BasicRejectAsync(
                321,
                false,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ProcessingThrows_NacksAndRequeuesMessage()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        var handler = CreateHandler(
            (_, _) => Task.FromException<bool>(
                new InvalidOperationException("Processing failed.")));

        // Act
        await handler.HandleAsync(
            CreateMessage(jobId),
            654,
            _channel.Object,
            CancellationToken.None);

        // Assert
        _channel.Verify(
            x => x.BasicNackAsync(
                654,
                false,
                true,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _channel.Verify(
            x => x.BasicAckAsync(
                It.IsAny<ulong>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Cancellation_DoesNotAcknowledgeOrReject()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        using var cancellationTokenSource =
            new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        var handler = CreateHandler(
            (_, token) => Task.FromCanceled<bool>(token));

        // Act
        await handler.HandleAsync(
            CreateMessage(jobId),
            987,
            _channel.Object,
            cancellationTokenSource.Token);

        // Assert
        _channel.Verify(
            x => x.BasicAckAsync(
                It.IsAny<ulong>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _channel.Verify(
            x => x.BasicNackAsync(
                It.IsAny<ulong>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _channel.Verify(
            x => x.BasicRejectAsync(
                It.IsAny<ulong>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}