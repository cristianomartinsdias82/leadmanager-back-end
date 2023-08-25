using CrossCutting.Messaging;
using System.Text;

namespace LeadManagerRemovedLead.Consumer;

public class RemovedLeadConsumerWorker : BackgroundService
{
    private readonly IMessageConsumption _messageConsumer;
    private readonly ILogger<RemovedLeadConsumerWorker> _logger;
    private readonly string _queueName;
    private readonly string _routingKey;

    public RemovedLeadConsumerWorker(
        IMessageConsumption messageConsumer,
        MessageChannelSettings messageChannelSettings,
        ILogger<RemovedLeadConsumerWorker> logger)
    {
        _messageConsumer = messageConsumer;
        _queueName = messageChannelSettings.RemovedLeadChannel.QueueName;
        _routingKey = messageChannelSettings.RemovedLeadChannel.RoutingKey;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        => await Task.Delay(0, stoppingToken);

    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1, stoppingToken);
        _logger.LogInformation("Starting Lead removal listener consumer worker...");

        _messageConsumer.Subscribe(
            ProcessIncomingData,
            _queueName,
            nameof(RemovedLeadConsumerWorker),
            default!);
    }

    private bool ProcessIncomingData(byte[] messageBytes)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        var data = Encoding.UTF8.GetString(messageBytes);

        Console.WriteLine("A Lead has been removed!");

        return true;
    }
}