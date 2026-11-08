using backend.Data;
using RabbitMQ.Client;

namespace backend.Services;

public class RabbitMqOutboxPublisher : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqOutboxPublisher> _logger;

    private readonly string _queueName;

    public RabbitMqOutboxPublisher(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<RabbitMqOutboxPublisher> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;

        _queueName = _configuration["RabbitMQ:Queue"]
            ?? "resume-analysis";
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var hostName = _configuration["RABBITMQ_HOST"]
                    ?? throw new InvalidOperationException("RABBITMQ_HOST is missing.");

                var factory = new ConnectionFactory
                {
                    HostName = hostName,

                    Port = int.TryParse(
                        _configuration["RABBITMQ_PORT"],
                        out var port) ? port : 5671,

                    UserName = _configuration["RABBITMQ_USERNAME"]
                        ?? throw new InvalidOperationException("RABBITMQ_USERNAME is missing."),

                    Password = _configuration["RABBITMQ_PASSWORD"]
                        ?? throw new InvalidOperationException("RABBITMQ_PASSWORD is missing."),

                    VirtualHost = _configuration["RABBITMQ_VHOST"]
                        ?? throw new InvalidOperationException("RABBITMQ_VHOST is missing."),

                    AutomaticRecoveryEnabled = true,

                    Ssl = new SslOption
                    {
                        Enabled = true,
                        ServerName = hostName
                    }
                };

                await using var connection =
                    await factory.CreateConnectionAsync(stoppingToken);

                var channelOptions = new CreateChannelOptions(
                    publisherConfirmationsEnabled: true,
                    publisherConfirmationTrackingEnabled: true
                );

                await using var channel =
                    await connection.CreateChannelAsync(
                        channelOptions,
                        stoppingToken);

                await channel.QueueDeclareAsync(
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: stoppingToken);

                _logger.LogInformation(
                    "RabbitMQ outbox publisher connected. Queue: {QueueName}",
                    _queueName);

                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = _scopeFactory.CreateScope();

                    var publisher = new OutboxMessagePublisher(
                        scope.ServiceProvider.GetRequiredService<AppDbContext>(),
                        scope.ServiceProvider.GetRequiredService<ILogger<OutboxMessagePublisher>>(),
                        _queueName);

                    await publisher.PublishPendingMessagesAsync(
                        channel,
                        stoppingToken);

                    await Task.Delay(
                        TimeSpan.FromSeconds(3),
                        stoppingToken);
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "RabbitMQ outbox publisher encountered an error. Retrying in 5 seconds.");

                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    stoppingToken);
            }
        }
    }
}
