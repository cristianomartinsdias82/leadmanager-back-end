using Microsoft.Extensions.DependencyInjection;

namespace ViaCep.ServiceClient.HealthChecking.Configuration;

public static class DependencyInjection
{
    public static IHealthChecksBuilder AddViaCepIntegration(this IHealthChecksBuilder healthCheckBuilder)
        => healthCheckBuilder.AddCheck<ViaCepIntegrationHealthCheck>("viacepserviceintegration");
}