using OpenTelemetry.Context.Propagation;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace CrossCutting.Messaging.RabbitMq.Diagnostics;

public static class RabbitMqDiagnostics
{
	public const string ActivitySourceName = "MessageBus";
	public static readonly ActivitySource ActivitySource = new ActivitySource(ActivitySourceName);

	public static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

	//	public static readonly Meter Meter = new(ActivityServiceName);

	//	public static Counter<long> CustomersCreatedCounter = Meter.CreateCounter<long>("clients.created");

	//	//public static System.Diagnostics.Metrics.UpDownCounter,
	//	//public static System.Diagnostics.Metrics.Gauge,
	//	//public static System.Diagnostics.Metrics.ObservableGauge,
	//	//public static System.Diagnostics.Metrics.ObservableCounter,
	//	//public static System.Diagnostics.Metrics.Histogram,
	//	//and so on...

	public const string ServiceName = "LeadManager.Api";
	public static readonly Meter Meter = new(ServiceName);

	public static Counter<long> RegisteredLeadsCounter = Meter.CreateCounter<long>(CommonConstants.Counters.RegisteredLeads);
	public static Counter<long> RemovedLeadsCounter = Meter.CreateCounter<long>(CommonConstants.Counters.RemovedLeads);
	public static Counter<long> UpdatedLeadsCounter = Meter.CreateCounter<long>(CommonConstants.Counters.UpdatedLeads);

	//public static System.Diagnostics.Metrics.UpDownCounter,
	//public static System.Diagnostics.Metrics.Gauge,
	//public static System.Diagnostics.Metrics.ObservableGauge,
	//public static System.Diagnostics.Metrics.ObservableCounter,
	//public static System.Diagnostics.Metrics.Histogram,
	//and so on...

	public static class CommonConstants
	{
		public const string LeadId = "lead.id";
		public const string LeadIds = "lead.ids";
		public const string HandlerName = "lead.handler.name";
		public const string ValueNotInformed = "not informed";
		public const string System = "rabbitmq";

		public static class Counters
		{
			public const string RegisteredLeads = "leads.created";
			public const string RemovedLeads = "leads.removed";
			public const string UpdatedLeads = "leads.updated";
		}
	}

	public static class MessageDispatcherConstants
	{
		public const string Operation = "publish";
		public const string DestinationKind_Queue = "queue";
		public const string DestinationKind_Exchange = "exchange";
		public const string ActivitySourceNameFormat = "Message bus activity ({0} - {1})";

		public const string MessagingDestinationName = "messaging.destination.name";
		public const string MessagingSystem = "messaging.system";
		public const string MessagingDestinationKind = "messaging.destination_kind";
		public const string MessagingOperation = "messaging.operation";
	}

	public static class MessageProducerConstants
	{
		public const string Operation = "process";
		public const string SourceKind_Queue = "queue";
		public const string ActivitySourceNameFormat = "Message bus activity ({0} - {1})";

		public const string MessagingSourceName = "messaging.source.name";
		public const string MessagingSystem = "messaging.system";
		public const string MessagingSourceKind = "messaging.source_kind";
		public const string MessagingOperation = "messaging.operation";

		public const string MessagingPayload = "messaging.payload";
		public const string MessagingPayload_Redacted = "### redacted ###";
	}
}
