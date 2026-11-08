using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;

namespace Backend.Tests.Services;

public class OutboxMessagePublisherTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly Mock<IChannel> _channel;
    private readonly Mock<ILogger<OutboxMessagePublisher>> _logger;
    private readonly OutboxMessagePublisher _publisher;

    public OutboxMessagePublisherTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();

        _channel = new Mock<IChannel>();
        _logger = new Mock<ILogger<OutboxMessagePublisher>>();

        _publisher = new OutboxMessagePublisher(
            _db,
            _logger.Object,
            "resume-analysis");
    }

    private async Task<OutboxMessage> CreateMessageAsync(
        string payload = "{\"jobId\":\"test\"}")
    {
        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "ResumeAnalysisRequested",
            Payload = payload,
            CreatedAt = DateTime.UtcNow
        };

        _db.OutboxMessages.Add(message);
        await _db.SaveChangesAsync();

        return message;
    }

    [Fact]
    public async Task PublishPendingMessages_Success_MarksMessageProcessed()
    {
        var message = await CreateMessageAsync();

        var count = await _publisher.PublishPendingMessagesAsync(
            _channel.Object,
            CancellationToken.None);

        Assert.Equal(1, count);
        Assert.NotNull(message.ProcessedAt);
        Assert.Null(message.NextAttemptAt);
        Assert.Null(message.LastError);

        _channel.Verify(x => x.BasicPublishAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<BasicProperties>(),
            It.IsAny<ReadOnlyMemory<byte>>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishPendingMessages_SetsPersistentMessageProperties()
    {
        var message = await CreateMessageAsync();

        BasicProperties? capturedProperties = null;
        string? capturedRoutingKey = null;

        _channel
            .Setup(x => x.BasicPublishAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, string, bool, BasicProperties,
                ReadOnlyMemory<byte>, CancellationToken>(
                (_, routingKey, _, properties, _, _) =>
                {
                    capturedProperties = properties;
                    capturedRoutingKey = routingKey;
                })
            .Returns(new ValueTask(
                Task.CompletedTask));

        await _publisher.PublishPendingMessagesAsync(
            _channel.Object,
            CancellationToken.None);

        Assert.NotNull(capturedProperties);
        Assert.True(capturedProperties!.Persistent);
        Assert.Equal("application/json", capturedProperties.ContentType);
        Assert.Equal(message.Id.ToString(), capturedProperties.MessageId);
        Assert.Equal("resume-analysis", capturedRoutingKey);
    }

    [Fact]
    public async Task PublishPendingMessages_Failure_IncrementsAttempts()
    {
        var message = await CreateMessageAsync();

        _channel
            .Setup(x => x.BasicPublishAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Broker unavailable"));

        var count = await _publisher.PublishPendingMessagesAsync(
            _channel.Object,
            CancellationToken.None);

        Assert.Equal(0, count);
        Assert.Equal(1, message.Attempts);
        Assert.Equal("Broker unavailable", message.LastError);
        Assert.NotNull(message.NextAttemptAt);
        Assert.Null(message.ProcessedAt);
    }

    [Fact]
    public async Task PublishPendingMessages_Failure_SchedulesExponentialRetry()
    {
        var message = await CreateMessageAsync();

        _channel
            .Setup(x => x.BasicPublishAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Failed"));

        var before = DateTime.UtcNow;

        await _publisher.PublishPendingMessagesAsync(
            _channel.Object,
            CancellationToken.None);

        var after = DateTime.UtcNow;

        Assert.InRange(
            message.NextAttemptAt!.Value,
            before.AddSeconds(2),
            after.AddSeconds(2));
    }

    [Fact]
    public async Task PublishPendingMessages_ProcessedMessage_DoesNotRepublish()
    {
        var message = await CreateMessageAsync();
        message.ProcessedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        var count = await _publisher.PublishPendingMessagesAsync(
            _channel.Object,
            CancellationToken.None);

        Assert.Equal(0, count);

        _channel.Verify(x => x.BasicPublishAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<BasicProperties>(),
            It.IsAny<ReadOnlyMemory<byte>>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task PublishPendingMessages_FutureRetry_DoesNotPublish()
    {
        var message = await CreateMessageAsync();
        message.NextAttemptAt = DateTime.UtcNow.AddMinutes(5);

        await _db.SaveChangesAsync();

        var count = await _publisher.PublishPendingMessagesAsync(
            _channel.Object,
            CancellationToken.None);

        Assert.Equal(0, count);

        _channel.Verify(x => x.BasicPublishAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<BasicProperties>(),
            It.IsAny<ReadOnlyMemory<byte>>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task PublishPendingMessages_ExceedingBatchSize_PublishesOnlyTen()
    {
        for (var i = 0; i < 12; i++)
        {
            await CreateMessageAsync($"{{\"number\":{i}}}");
        }

        var count = await _publisher.PublishPendingMessagesAsync(
            _channel.Object,
            CancellationToken.None);

        Assert.Equal(10, count);

        Assert.Equal(
            10,
            await _db.OutboxMessages.CountAsync(
                m => m.ProcessedAt != null));

        _channel.Verify(x => x.BasicPublishAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<BasicProperties>(),
            It.IsAny<ReadOnlyMemory<byte>>(),
            It.IsAny<CancellationToken>()),
            Times.Exactly(10));
    }

    [Fact]
    public async Task PublishPendingMessages_Cancellation_ThrowsOperationCanceledException()
    {
        await CreateMessageAsync();

        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _publisher.PublishPendingMessagesAsync(
                _channel.Object,
                cancellation.Token));
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}