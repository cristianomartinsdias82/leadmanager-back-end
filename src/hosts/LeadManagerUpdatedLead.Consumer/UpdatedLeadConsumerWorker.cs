using CrossCutting.Messaging;
using System.Text;

namespace LeadManagerUpdatedLead.Consumer;

public class UpdatedLeadConsumerWorker : BackgroundService
{
    private readonly IMessageConsumption _messageConsumer;
    private readonly ILogger<UpdatedLeadConsumerWorker> _logger;
    private readonly string _queueName;
    private readonly string _routingKey;

    public UpdatedLeadConsumerWorker(
        IMessageConsumption messageConsumer,
        MessageChannelSettings messageChannelSettings,
        ILogger<UpdatedLeadConsumerWorker> logger)
    {
        _messageConsumer = messageConsumer;
        _queueName = messageChannelSettings.UpdatedLeadChannel.QueueName;
        _routingKey = messageChannelSettings.UpdatedLeadChannel.RoutingKey;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        => await Task.Delay(0, stoppingToken);

    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1, stoppingToken);
        _logger.LogInformation("Starting Lead update listener consumer worker...");

        _messageConsumer.Subscribe(
            ProcessIncomingData,
            _queueName,
            nameof(UpdatedLeadConsumerWorker),
            default!);
    }

    private bool ProcessIncomingData(byte[] messageBytes)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        var data = Encoding.UTF8.GetString(messageBytes);

        Console.WriteLine("A Lead has been updated!");

        return true;
    }
}