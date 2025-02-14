using CrossCutting.Messaging.RabbitMq.Diagnostics;
using CrossCutting.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using RabbitMQ.Client.Events;
using Shared.Diagnostics;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static CrossCutting.Messaging.RabbitMq.Diagnostics.RabbitMqDiagnostics;

namespace CrossCutting.Messaging.RabbitMq;

internal sealed class RabbitMqMessageConsumer : MessageConsumer
{
    private readonly IRabbitMqChannelFactory _rabbitMqChannelFactory;
    private readonly ILogger<RabbitMqMessageConsumer> _logger;
    private readonly IHostEnvironment _hostEnvironment;
	private readonly IDataSerialization _dataSerializer;

	public RabbitMqMessageConsumer(
        IRabbitMqChannelFactory rabbitMqChannelFactory,
		IDataSerialization dataSerializer,
		IHostEnvironment hostEnvironment,
        ILogger<RabbitMqMessageConsumer> logger)
    {
        _rabbitMqChannelFactory = rabbitMqChannelFactory;
		_dataSerializer = dataSerializer;
        _hostEnvironment = hostEnvironment;
		_logger = logger;
    }

	public override void Subscribe<T>(
		Func<byte[], IEnumerable<ActivityLink>?, bool> onMessageReceived,
		string queueName,
		string consumerIdentifier,
		IDictionary<string, object> arguments)
	{
		var channel = _rabbitMqChannelFactory.CreateChannel();
		var consumer = new EventingBasicConsumer(channel);

		consumer.Received += (_, ea) =>
		{
			var acknowledge = false;

			try
			{
				var activityLinks = PushTelemetryData<T>(ea);

				acknowledge = onMessageReceived(ea.Body.ToArray(), activityLinks);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Error when receiving message from queue {queueName} for consumer {consumerIdentifier}", queueName, consumerIdentifier);
			}
			finally
			{
				if (acknowledge)
					channel.BasicAck(ea.DeliveryTag, false);
				else
					channel.BasicNack(ea.DeliveryTag, false, true);
			}
		};

		channel.BasicConsume(queueName,
							 false,
							 consumerIdentifier,
							 true,
							 false,
							 arguments,
							 consumer);
	}

	private IEnumerable<ActivityLink>? PushTelemetryData<T>(BasicDeliverEventArgs ea)
	{
		//Extracting the context propagation telemetry data
		var parentContext = RabbitMqDiagnostics
								.Propagator
								.Extract(
									context: default,
									ea.BasicProperties,
									(props, key) =>
									{
										if (!props.Headers.TryGetValue(key, out var value))
											return [];

										var bytes = (value as byte[])!;

										var temp = Encoding.UTF8.GetString(bytes);

										return [Encoding.UTF8.GetString(bytes)];
									});

		Baggage.Current = parentContext.Baggage;

		//Start an activity with a name following the semantic convention of the Open Telemetry Protocol as per the link below:
		//https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/rabbitmq.md
		var diagnostics = DiagnosticsDataCollector.WithActivityFromSource(
							source: RabbitMqDiagnostics.ActivitySource,
							name: string.Format(MessageProducerConstants.ActivitySourceNameFormat, ea.RoutingKey, MessageProducerConstants.Operation),
							kind: ActivityKind.Consumer,
							parentContext.ActivityContext);
		
		//It helps visualizing the RabbitMq participation in the traces
		//These tags are added demonstrating the semantic conventions of the OpenTelemetry protocol
		//https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/rabbitmq.md
		diagnostics.WithTags(
			(MessageProducerConstants.MessagingSourceName, /*ea!*/GetType().Name),
			(MessageProducerConstants.MessagingSystem, CommonConstants.System),
			(MessageProducerConstants.MessagingSourceKind, MessageProducerConstants.SourceKind_Queue),
			(MessageProducerConstants.MessagingOperation, MessageProducerConstants.Operation),

			(MessageProducerConstants.MessagingPayload, _hostEnvironment.IsProduction() ? MessageProducerConstants.MessagingPayload_Redacted : JsonSerializer.Serialize(_dataSerializer.Deserialize<T>(ea.Body.ToArray()))),
			(CommonConstants.LeadId, Baggage.Current.GetBaggage(CommonConstants.LeadId) ?? CommonConstants.ValueNotInformed),
			(CommonConstants.LeadIds, Baggage.Current.GetBaggage(CommonConstants.LeadIds) ?? CommonConstants.ValueNotInformed)
		)
		.PushData();

		return [new ActivityLink(parentContext.ActivityContext)];
	}
}