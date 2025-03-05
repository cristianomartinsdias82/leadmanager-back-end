using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
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
                sp => sp.GetRequiredService<IMongoClient>(),
                timeout: TimeSpan.FromSeconds(dataSourceSettings.HealthCheckingTimeoutInSecs));
    }
}
