using CrossCutting.Monitoring.IAMServer.HealthChecking.Configuration;
using CrossCutting.Monitoring.IAMServer.Metrics.Configuration;
using CrossCutting.Monitoring.LeadManagerApi.CustomMetrics.Configuration;
using CrossCutting.Monitoring.LeadManagerApi.HealthChecking.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.Monitoring.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddLeadManagerApiMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLeadManagerApiMetrics(configuration)
                .AddHealthChecks()
                .AddLeadManagerApiHealthChecks(configuration);

        return services;
    }

    public static IServiceCollection AddIAMServerMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIAMServerMetrics(configuration)
                .AddHealthChecks()
                .AddIAMServerHealthChecks(configuration);

        return services;
    }
}