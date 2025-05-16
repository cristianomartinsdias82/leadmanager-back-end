using CrossCutting.MessageContracts;
using CrossCutting.Messaging;
using CrossCutting.Messaging.RabbitMq.Diagnostics;
using CrossCutting.Serialization;
using LeadManager.BackendServices.Consumers.Common.Diagnostics;
using Shared.Diagnostics;
using System.Diagnostics;
using System.Text;

namespace LeadManagerUpdatedLead.Consumer;

public class UpdatedLeadConsumerWorker : BackgroundService
{
    private readonly IMessageConsumption _messageConsumer;
	private readonly IDataSerialization _dataSerializer;
	private readonly ILogger<UpdatedLeadConsumerWorker> _logger;
	private readonly TimeProvider _timeProvider;
	private readonly string _queueName;
    private readonly string _routingKey;

    public UpdatedLeadConsumerWorker(
        IMessageConsumption messageConsumer,
        MessageChannelSettings messageChannelSettings,
		IDataSerialization dataSerializer,
		TimeProvider timeProvider,
        ILogger<UpdatedLeadConsumerWorker> logger)
    {
        _messageConsumer = messageConsumer;
        _queueName = messageChannelSettings.UpdatedLeadChannel.QueueName;
		_dataSerializer = dataSerializer;
		_timeProvider = timeProvider;
        _routingKey = messageChannelSettings.UpdatedLeadChannel.RoutingKey;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        => await Task.Delay(0, stoppingToken);

    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1, stoppingToken);
        _logger.LogInformation("Starting Lead update listener consumer worker...");

        _messageConsumer.Subscribe<string>(
            ProcessIncomingData,
            _queueName,
            nameof(UpdatedLeadConsumerWorker),
            default!);
    }

    private bool ProcessIncomingData(byte[] messageBytes, IEnumerable<ActivityLink>? activityLinks)
	{
		_logger.LogInformation("A lead has been updated!");

		var updatedLead = _dataSerializer.Deserialize<LeadData>(messageBytes);

		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("A lead has been updated!");
		Console.WriteLine(updatedLead);
		Console.ResetColor();

		DiagnosticsDataCollector
			.WithActivityFromSource(
				source: RabbitMqDiagnostics.ActivitySource,
				name: MessageConsumersDiagnostics.UpdatedLeadConstants.ActivityName,
				kind: ActivityKind.Internal,
				context: new ActivityContext(),
				links: activityLinks //This last argument links the activities collected inside "(ExecuteTaskAsync method) consumer.Received += (model, ea) => {...}" to this activity.
			)
			.WithTags
			(
				(MessageConsumersDiagnostics.UpdatedLeadConstants.LeadId, updatedLead.Id)
			)
			.WithEvent(
				name: MessageConsumersDiagnostics.UpdatedLeadConstants.ActivityName,
				timestamp: _timeProvider.GetLocalNow(),
				tags: [
					(MessageConsumersDiagnostics.CommonConstants.LeadProcessResultSucessful, "true")
				])
			//By using .WithEvent, the information appears as a subsection inside of the Span labeled "Logs" in the Tracing Web tool
			.PushData();

		return true;
	}
}