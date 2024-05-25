using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace CrossCutting.Monitoring.IAMServer.Metrics.Configuration;

internal static class DependencyInjection
{
    public static IServiceCollection AddIAMServerMetrics(this IServiceCollection services, IConfiguration configuration)
        => services.UseHttpClientMetrics();
}