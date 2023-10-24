using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CrossCutting.Monitoring.LeadManagerApi.HealthChecking.Configuration;
using CrossCutting.Monitoring.IAMServer.HealthChecking.Configuration;

namespace CrossCutting.Monitoring.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
                .AddLeadManagerApiHealthChecks(configuration)
                .AddIAMServerHealthChecks(configuration);

        return services;
    }
}