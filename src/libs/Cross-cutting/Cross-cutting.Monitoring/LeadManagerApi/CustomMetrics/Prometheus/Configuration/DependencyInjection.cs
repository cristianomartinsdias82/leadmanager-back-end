using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace CrossCutting.Monitoring.LeadManagerApi.CustomMetrics.Prom.Configuration;

internal static class DependencyInjection
{
    public static IServiceCollection AddPrometheusMetricsCollectors(this IServiceCollection services, IConfiguration configuration)
        => services.UseHttpClientMetrics()
                   .AddSingleton<ILeadManagerApiMetricsCollection, LeadManagerApiMetricsCollector>();
}