using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CrossCutting.Monitoring.LeadManagerApi.HealthChecking.Configuration;
using CrossCutting.Monitoring.IAMServer.HealthChecking.Configuration;

namespace CrossCutting.Monitoring.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddLeadManagerApiMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
                .AddLeadManagerApiHealthChecks(configuration);

        return services;
    }

    public static IServiceCollection AddIAMServerMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
                .AddIAMServerHealthChecks(configuration);

        return services;
    }
}