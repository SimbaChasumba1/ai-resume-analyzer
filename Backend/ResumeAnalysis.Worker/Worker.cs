using backend.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ResumeAnalysis.Worker;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<Worker> _logger;

    private readonly string _queueName;

    public Worker(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<Worker> logger)
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
                    ?? throw new InvalidOperationException(
                        "RABBITMQ_HOST is missing.");

                var factory = new ConnectionFactory
                {
                    HostName = hostName,

                    Port = int.TryParse(
                        _configuration["RABBITMQ_PORT"],
                        out var port) ? port : 5671,

                    UserName = _configuration["RABBITMQ_USERNAME"]
                        ?? throw new InvalidOperationException(
                            "RABBITMQ_USERNAME is missing."),

                    Password = _configuration["RABBITMQ_PASSWORD"]
                        ?? throw new InvalidOperationException(
                            "RABBITMQ_PASSWORD is missing."),

                    VirtualHost = _configuration["RABBITMQ_VHOST"]
                        ?? throw new InvalidOperationException(
                            "RABBITMQ_VHOST is missing."),

                    AutomaticRecoveryEnabled = true,

                    Ssl = new SslOption
                    {
                        Enabled = true,
                        ServerName = hostName
                    }
                };

                await using var connection =
                    await factory.CreateConnectionAsync(stoppingToken);

                await using var channel =
                    await connection.CreateChannelAsync(
                        cancellationToken: stoppingToken);

                await channel.QueueDeclareAsync(
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: stoppingToken);

                await channel.BasicQosAsync(
                    prefetchSize: 0,
                    prefetchCount: 1,
                    global: false,
                    cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(channel);

               var messageHandler = new WorkerMessageHandler(
                ProcessJobAsync,
                LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<WorkerMessageHandler>());

                consumer.ReceivedAsync += async (_, eventArgs) =>
                {
                    await messageHandler.HandleAsync(
                        eventArgs.Body,
                        eventArgs.DeliveryTag,
                        channel,
                        stoppingToken);
                };

                await channel.BasicConsumeAsync(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                _logger.LogInformation(
                    "Listening for messages on queue {QueueName}.",
                    _queueName);

                await Task.Delay(
                    Timeout.Infinite,
                    stoppingToken);
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
                    "RabbitMQ connection failed. Retrying in 5 seconds.");

                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    stoppingToken);
            }
        }
    }

    private async Task<bool> ProcessJobAsync(
        Guid jobId,
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var processor = scope.ServiceProvider
            .GetRequiredService<AnalysisJobProcessor>();

        return await processor.ProcessJobAsync(
            jobId,
            cancellationToken);
    }
}
