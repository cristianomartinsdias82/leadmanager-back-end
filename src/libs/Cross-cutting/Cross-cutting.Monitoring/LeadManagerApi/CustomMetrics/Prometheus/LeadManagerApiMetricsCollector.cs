using Microsoft.Extensions.Logging;
using Prometheus;

namespace CrossCutting.Monitoring.LeadManagerApi.CustomMetrics.Prom;

internal sealed class LeadManagerApiMetricsCollector : ILeadManagerApiMetricsCollection
{
    private const string IncrementRegisteredLeadsCounterName = "total_registered_leads";
    private static readonly Counter RegisteredLeadsCount = Metrics.CreateCounter(IncrementRegisteredLeadsCounterName, "The total of Leads registered in the system.");
    private readonly ILogger<LeadManagerApiMetricsCollector> _logger;

    public LeadManagerApiMetricsCollector(ILogger<LeadManagerApiMetricsCollector> logger)
        => _logger = logger;

    public Task IncrementRegisteredLeadsCounter()
        => Task.Run(() => {
            try
            {
                RegisteredLeadsCount.Inc();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error while trying to feed metrics collector's " + "'" + IncrementRegisteredLeadsCounterName + "'" + " counter. Message: {message}", e.Message);
            }
        });
}
