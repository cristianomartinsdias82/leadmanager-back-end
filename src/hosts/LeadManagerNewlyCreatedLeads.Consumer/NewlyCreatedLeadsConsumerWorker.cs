using CrossCutting.Messaging;
using System.Text;

namespace LeadManagerNewlyCreatedLeads.Consumer;

public class NewlyCreatedLeadsConsumerWorker : BackgroundService
{
    private readonly IMessageConsumption _messageConsumer;
    private readonly ILogger<NewlyCreatedLeadsConsumerWorker> _logger;
    private readonly string _queueName;
    private readonly string _routingKey;

    public NewlyCreatedLeadsConsumerWorker(
        IMessageConsumption messageConsumer,
        MessageChannelSettings messageChannelSettings,
        ILogger<NewlyCreatedLeadsConsumerWorker> logger)
    {
        _messageConsumer = messageConsumer;
        _queueName = messageChannelSettings.NewlyRegisteredLeadsChannel.QueueName;
        _routingKey = messageChannelSettings.NewlyRegisteredLeadsChannel.RoutingKey;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        => await Task.Delay(0, stoppingToken);

    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1, stoppingToken);
        _logger.LogInformation("Starting Lead(s) registration listener consumer worker...");

        _messageConsumer.Subscribe(
            ProcessIncomingData,
            _queueName,
            nameof(NewlyCreatedLeadsConsumerWorker),
            default!);
    }

    private bool ProcessIncomingData(byte[] messageBytes)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        var data = Encoding.UTF8.GetString(messageBytes);

        Console.WriteLine("New leads have been registered!");

        return true;
    }
}