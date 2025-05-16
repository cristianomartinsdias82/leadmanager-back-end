using CrossCutting.MessageContracts;
using CrossCutting.Messaging;
using CrossCutting.Messaging.RabbitMq.Diagnostics;
using CrossCutting.Serialization;
using LeadManager.BackendServices.Consumers.Common.Diagnostics;
using Shared.Diagnostics;
using System.Diagnostics;

namespace LeadManagerRemovedLead.Consumer;

public class RemovedLeadConsumerWorker : BackgroundService
{
    private readonly IMessageConsumption _messageConsumer;
	private readonly IDataSerialization _dataSerializer;
	private readonly TimeProvider _timeProvider;
	private readonly ILogger<RemovedLeadConsumerWorker> _logger;
    private readonly string _queueName;
    private readonly string _routingKey;

    public RemovedLeadConsumerWorker(
        IMessageConsumption messageConsumer,
        MessageChannelSettings messageChannelSettings,
		IDataSerialization dataSerializer,
        TimeProvider timeProvider,
		ILogger<RemovedLeadConsumerWorker> logger)
    {
        _messageConsumer = messageConsumer;
		_dataSerializer = dataSerializer;
		_timeProvider = timeProvider;
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

        _messageConsumer.Subscribe<string>(
            ProcessIncomingData,
            _queueName,
            nameof(RemovedLeadConsumerWorker),
            default!);
    }

    private bool ProcessIncomingData(byte[] messageBytes, IEnumerable<ActivityLink>? activityLinks)
	{
		_logger.LogInformation("A lead has been removed!");

		var removedLead = _dataSerializer.Deserialize<LeadData>(messageBytes);

		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine("A lead has been removed!");
		Console.WriteLine(removedLead);
		Console.ResetColor();

		DiagnosticsDataCollector
			.WithActivityFromSource(
				source: RabbitMqDiagnostics.ActivitySource,
				name: MessageConsumersDiagnostics.RemovedLeadConstants.ActivityName,
				kind: ActivityKind.Internal,
				context: new ActivityContext(),
				links: activityLinks //This last argument links the activities collected inside "(ExecuteTaskAsync method) consumer.Received += (model, ea) => {...}" to this activity.
			)
			.WithTags
			(
				(MessageConsumersDiagnostics.RemovedLeadConstants.LeadId, removedLead.Id)
			)
			.WithEvent(
				name: MessageConsumersDiagnostics.RemovedLeadConstants.ActivityName,
				timestamp: _timeProvider.GetLocalNow(),
				tags: [
					(MessageConsumersDiagnostics.CommonConstants.LeadProcessResultSucessful, "true")
				])
			//By using .WithEvent, the information appears as a subsection inside of the Span labeled "Logs" in the Tracing Web tool
			.PushData();

        return true;
    }
}