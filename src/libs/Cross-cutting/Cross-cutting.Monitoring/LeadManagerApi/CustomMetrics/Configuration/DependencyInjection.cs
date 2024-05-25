using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CrossCutting.Monitoring.LeadManagerApi.CustomMetrics.Prom.Configuration;

namespace CrossCutting.Monitoring.LeadManagerApi.CustomMetrics.Configuration;

internal static class DependencyInjection
{
    public static IServiceCollection AddLeadManagerApiMetrics(this IServiceCollection services, IConfiguration configuration)
        => services.AddPrometheusMetricsCollectors(configuration);
}