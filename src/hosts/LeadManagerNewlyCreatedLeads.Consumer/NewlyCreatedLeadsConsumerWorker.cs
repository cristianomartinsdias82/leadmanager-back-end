using CrossCutting.MessageContracts;
using CrossCutting.Messaging;
using CrossCutting.Messaging.RabbitMq.Diagnostics;
using CrossCutting.Serialization;
using LanguageExt.Pipes;
using LeadManager.BackendServices.Consumers.Common.Diagnostics;
using OpenTelemetry;
using Shared.Diagnostics;
using System.Diagnostics;

namespace LeadManagerNewlyCreatedLeads.Consumer;

public class NewlyCreatedLeadsConsumerWorker : BackgroundService
{
    private readonly IMessageConsumption _messageConsumer;
	private readonly IDataSerialization _dataSerializer;
	private readonly TimeProvider _timeProvider;
	private readonly ILogger<NewlyCreatedLeadsConsumerWorker> _logger;
    private readonly string _queueName;
    private readonly string _routingKey;

    public NewlyCreatedLeadsConsumerWorker(
        IMessageConsumption messageConsumer,
        IDataSerialization dataSerializer,
        MessageChannelSettings messageChannelSettings,
        TimeProvider timeProvider,
        ILogger<NewlyCreatedLeadsConsumerWorker> logger)
    {
        _messageConsumer = messageConsumer;
		_dataSerializer = dataSerializer;
		_timeProvider = timeProvider;
		_queueName = messageChannelSettings.NewlyRegisteredLeadsChannel.QueueName;
        _routingKey = messageChannelSettings.NewlyRegisteredLeadsChannel.RoutingKey;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        => await Task.Delay(0, stoppingToken);

    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(0, stoppingToken);
        _logger.LogInformation("Starting Lead(s) registration listener consumer worker...");

        _messageConsumer.Subscribe<IEnumerable<LeadData>>(
            ProcessIncomingData,
            _queueName,
            nameof(NewlyCreatedLeadsConsumerWorker),
            default!);
    }

    private bool ProcessIncomingData(byte[] messageBytes, IEnumerable<ActivityLink>? activityLinks)
    {
        _logger.LogInformation("New leads have been registered!");

        var incomingLeads = _dataSerializer.Deserialize<IEnumerable<LeadData>>(messageBytes);

		Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("New leads have been registered!");
        incomingLeads
            .ToList()
            .ForEach(ld => Console.WriteLine(ld));

		Console.ResetColor();

        var diagnostics = DiagnosticsDataCollector
            .WithActivityFromSource(
                source: RabbitMqDiagnostics.ActivitySource,
                name: MessageConsumersDiagnostics.NewlyCreatedLeadsConstants.ActivityName,
                kind: ActivityKind.Internal,
                context: new ActivityContext(),
                links: activityLinks //This last argument links the activities collected inside "(ExecuteTaskAsync method) consumer.Received += (model, ea) => {...}" to this activity.
            )
            .WithTags
            (
                (MessageConsumersDiagnostics.NewlyCreatedLeadsConstants.Count, incomingLeads.Count()),
                (MessageConsumersDiagnostics.NewlyCreatedLeadsConstants.LeadIds, string.Join(",", incomingLeads.Select(ld => ld.Id))),
                //[..Baggage.Current.GetBaggage().Select(kvp => new (kvp.Key, kvp.Value))]
            );

		var baggageEntries = Baggage.Current.GetBaggage();
		foreach (var entry in baggageEntries)
			diagnostics.WithTags(($"Baggage data being consumed inside {GetType().Name} consumer class: {entry.Key}", entry.Value));

		diagnostics
            .WithEvent(
                name: MessageConsumersDiagnostics.NewlyCreatedLeadsConstants.ActivityName,
                timestamp: _timeProvider.GetUtcNow(),
                tags: [
                    (MessageConsumersDiagnostics.CommonConstants.LeadProcessResultSucessful, "true")
                ])
            //By using .WithEvent, the information appears as a subsection inside of the Span labeled "Logs" in the Tracing Web tool
            .PushData();

		return true;
    }
}