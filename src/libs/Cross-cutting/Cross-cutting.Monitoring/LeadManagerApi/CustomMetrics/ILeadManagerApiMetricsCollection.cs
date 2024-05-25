namespace CrossCutting.Monitoring.LeadManagerApi.CustomMetrics;

public interface ILeadManagerApiMetricsCollection
{
    Task IncrementRegisteredLeadsCounter();
}
