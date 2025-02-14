using System.Diagnostics.Metrics;

namespace Application.Core.Diagnostics;

public static class ApplicationDiagnostics
{
	public const string ServiceName = "LeadManager.Api";
	public static readonly Meter Meter = new(ServiceName);

	public static Counter<long> RegisteredLeadsCounter = Meter.CreateCounter<long>(Constants.Counters.RegisteredLeads);
	public static Counter<long> RemovedLeadsCounter = Meter.CreateCounter<long>(Constants.Counters.RemovedLeads);
	public static Counter<long> UpdatedLeadsCounter = Meter.CreateCounter<long>(Constants.Counters.UpdatedLeads);

	//public static System.Diagnostics.Metrics.UpDownCounter,
	//public static System.Diagnostics.Metrics.Gauge,
	//public static System.Diagnostics.Metrics.ObservableGauge,
	//public static System.Diagnostics.Metrics.ObservableCounter,
	//public static System.Diagnostics.Metrics.Histogram,
	//and so on...

	public static class Constants
	{
		public const string LeadId = "lead.id";
		public const string LeadIds = "lead.ids";
		public const string HandlerName = "lead.handler.name";

		public static class Counters
		{
			public const string RegisteredLeads = "leads.created";
			public const string RemovedLeads = "leads.removed";
			public const string UpdatedLeads = "leads.updated";
		}
	}
}
