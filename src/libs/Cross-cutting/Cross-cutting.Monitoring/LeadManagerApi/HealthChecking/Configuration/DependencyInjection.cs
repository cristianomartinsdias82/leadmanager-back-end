using CrossCutting.Caching.Redis.Configuration;
using CrossCutting.FileStorage.Azure.Configuration;
using CrossCutting.Messaging.RabbitMq.Configuration;
using ViaCep.ServiceClient.HealthChecking.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Settings;

namespace CrossCutting.Monitoring.LeadManagerApi.HealthChecking.Configuration;

public static class DependencyInjection
{
    public static IHealthChecksBuilder AddLeadManagerApiHealthChecks(this IHealthChecksBuilder healthChecksBuilder, IConfiguration configuration)
    {
        var storageProviderSettings = configuration.GetSection(nameof(AzureStorageProviderSettings))
                                                   .Get<AzureStorageProviderSettings>()!;

        var cachingProviderSettings = configuration.GetSection(nameof(RedisCacheProviderSettings))
                                                   .Get<RedisCacheProviderSettings>()!;

        var dataSourceSettings = configuration.GetSection(nameof(DataSourceSettings))
                                              .Get<DataSourceSettings>()!;

        var rabbitMqConnectionSettings = configuration.GetSection(nameof(RabbitMqConnectionSettings))
                                                      .Get<RabbitMqConnectionSettings>()!;

        healthChecksBuilder
            .AddViaCepIntegration()
            .AddAzureBlobStorage
            (
                connectionString: storageProviderSettings.ConnectionString,
                containerName: storageProviderSettings.ContainerName,
                timeout: TimeSpan.FromSeconds(storageProviderSettings.HealthCheckingTimeoutInSecs)
            )
            .AddRedis(redisConnectionString: $"{cachingProviderSettings.Server}:{cachingProviderSettings.PortNumber}",
                      timeout: TimeSpan.FromSeconds(cachingProviderSettings.HealthCheckingTimeoutInSecs))
            .AddSqlServer(connectionString: dataSourceSettings.ConnectionString,
                      timeout: TimeSpan.FromSeconds(cachingProviderSettings.HealthCheckingTimeoutInSecs))
            .AddRabbitMQ(rabbitConnectionString: rabbitMqConnectionSettings.ConnectionString,
                      timeout: TimeSpan.FromSeconds(cachingProviderSettings.HealthCheckingTimeoutInSecs));

        return healthChecksBuilder;
    }
}
