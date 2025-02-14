using CrossCutting.Messaging.RabbitMq.Diagnostics;
using CrossCutting.Serialization;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using Shared.Diagnostics;
using System.Diagnostics;
using static CrossCutting.Messaging.RabbitMq.Diagnostics.RabbitMqDiagnostics;

namespace CrossCutting.Messaging.RabbitMq;

internal sealed class RabbitMqMessageDispatcher : MessageDispatcher
{
    private readonly IRabbitMqChannelFactory _rabbitMqChannelFactory;
    private readonly IDataSerialization _dataSerializer;

    public RabbitMqMessageDispatcher(
		IRabbitMqChannelFactory rabbitMqChannelFactory,
		IDataSerialization dataSerializer)
    {
        _rabbitMqChannelFactory = rabbitMqChannelFactory;
        _dataSerializer = dataSerializer;
    }

	public override ValueTask SendToTopicAsync<T>(
		string topicName,
		string routingKey,
		T data,
		string eventType,
		CancellationToken cancellationToken = default)
	{
		using var channel = _rabbitMqChannelFactory.CreateChannel();

		var contextPropagationProperties = PropagateTelemetryData(
											channel,
											operation: MessageDispatcherConstants.Operation,
											eventType);

		channel.BasicPublish(
			exchange: topicName,
			routingKey: routingKey,
			basicProperties: contextPropagationProperties,
			body: _dataSerializer.Serialize(data!));

		return new();
	}

	public override ValueTask SendToQueueAsync<T>(
        string queueName,
        T data,
		string eventType,
		CancellationToken cancellationToken = default)
    {
        using var channel = _rabbitMqChannelFactory.CreateChannel();

		var contextPropagationProperties = PropagateTelemetryData(
											channel,
											operation: MessageDispatcherConstants.Operation,
											eventType);

		channel.BasicPublish(
            string.Empty,
            queueName,
			basicProperties: contextPropagationProperties,
			_dataSerializer.Serialize(data!));

        return new();
    }

    public override ValueTask SendToQueueAsync(
        string queueName,
        byte[] bytes,
		string eventType,
		CancellationToken cancellationToken = default)
    {
        using var channel = _rabbitMqChannelFactory.CreateChannel();

		var contextPropagationProperties = PropagateTelemetryData(
											channel,
											operation: MessageDispatcherConstants.Operation,
											eventType,
                                            destinationKind: MessageDispatcherConstants.DestinationKind_Queue);

		channel.BasicPublish(
            string.Empty,
            queueName,
			basicProperties: contextPropagationProperties,
			bytes);

        return new();
    }

    public override ValueTask SendToTopicAsync(
        string topicName,
        string routingKey,
        byte[] bytes,
		string eventType,
		CancellationToken cancellationToken = default)
    {
        using var channel = _rabbitMqChannelFactory.CreateChannel();

		var contextPropagationProperties = PropagateTelemetryData(
		                                    channel,
											operation: MessageDispatcherConstants.Operation,
											eventType);

		channel.BasicPublish(
            topicName,
            routingKey,
			basicProperties: contextPropagationProperties,
			bytes);

        return new();
    }

	private IBasicProperties PropagateTelemetryData(
		IModel channel,
		string operation,
		string eventType,
		string destinationKind = MessageDispatcherConstants.DestinationKind_Exchange)
	{
		//Start an activity with a name following the semantic convention of the Open Telemetry Protocol as per the link below:
		//https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/rabbitmq.md
		var diagnostics = DiagnosticsDataCollector.WithActivityFromSource(
			RabbitMqDiagnostics.ActivitySource,
			name: string.Format(MessageDispatcherConstants.ActivitySourceNameFormat, eventType, operation),
			kind: ActivityKind.Producer,
			context: null);

		ActivityContext contextToInject = diagnostics.Activity is not null ? diagnostics.Activity.Context : Activity.Current is not null ? Activity.Current.Context : default;

		var contextPropagationProperties = channel.CreateBasicProperties();
		contextPropagationProperties.DeliveryMode = (byte)RabbitMqDeliveryMode.Persistent;

		RabbitMqDiagnostics.Propagator
							.Inject(context: new PropagationContext(contextToInject, Baggage.Current),
									carrier: contextPropagationProperties,
									setter: (props, key, value) =>
									{
										props.Headers ??= new Dictionary<string, object>();
										props.Headers.Add(key, value);

										var headers = props.Headers;
									});

		//It helps visualizing the RabbitMq participation in the traces
		//These tags are added demonstrating the semantic conventions of the OpenTelemetry protocol
		//https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/rabbitmq.md
		diagnostics.WithTags(
			(MessageDispatcherConstants.MessagingDestinationName, eventType),
			(MessageDispatcherConstants.MessagingSystem, CommonConstants.System),
			(MessageDispatcherConstants.MessagingDestinationKind, destinationKind),
			(MessageDispatcherConstants.MessagingOperation, operation),
			(CommonConstants.LeadId, Baggage.Current.GetBaggage(CommonConstants.LeadId) ?? CommonConstants.ValueNotInformed),
			(CommonConstants.LeadIds, Baggage.Current.GetBaggage(CommonConstants.LeadIds) ?? CommonConstants.ValueNotInformed)
		)
		.PushData();

		return contextPropagationProperties;
	}
}
