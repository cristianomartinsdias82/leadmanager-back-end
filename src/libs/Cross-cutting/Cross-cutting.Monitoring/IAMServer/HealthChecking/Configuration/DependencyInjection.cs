using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Settings;

namespace CrossCutting.Monitoring.IAMServer.HealthChecking.Configuration;

internal static class DependencyInjection
{
    public static IHealthChecksBuilder AddIAMServerHealthChecks(this IHealthChecksBuilder healthChecksBuilder, IConfiguration configuration)
    {
        var dataSourceSettings = configuration.GetSection(nameof(DataSourceSettings))
                                              .Get<DataSourceSettings>()!;

        return healthChecksBuilder
            .AddMongoDb(
                mongodbConnectionString: dataSourceSettings.ConnectionString,
                timeout: TimeSpan.FromSeconds(dataSourceSettings.HealthCheckingTimeoutInSecs));
    }
}
