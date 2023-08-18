using Application.Contracts.Caching;
using Application.Contracts.Caching.Policies;
using Application.Contracts.Persistence;
using Infrastructure.Caching;
using Infrastructure.EventDispatching;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Events.EventDispatching;
using Shared.Settings;

namespace Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        => services.AddDataSource(configuration)
                   .AddEventDispatcher(configuration)
                   .AddCacheManager(configuration);

    private static IServiceCollection AddDataSource(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSourceSettings = configuration.GetSection(nameof(DataSourceSettings)).Get<DataSourceSettings>()!;
        services.AddSingleton(dataSourceSettings);

        services.AddDbContext<LeadManagerDbContext>(config =>
        {
            config.UseSqlServer(
                dataSourceSettings.ConnectionString,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(dataSourceSettings.RetryOperationMaxCount);
                });
        });

        services.AddScoped<ILeadManagerDbContext>(
            services => services.GetRequiredService<LeadManagerDbContext>()
        );

        return services;
    }

    private static IServiceCollection AddEventDispatcher(this IServiceCollection services, IConfiguration configuration)
        => services.AddScoped<IEventDispatching, EventDispatcher>();

    private static IServiceCollection AddCacheManager(this IServiceCollection services, IConfiguration configuration)
    {
        var cachingPoliciesSettings = configuration.GetSection(nameof(CachingPoliciesSettings)).Get<CachingPoliciesSettings>()!;
        services.AddSingleton(cachingPoliciesSettings);

        return services.AddScoped<ICachingManagement, CacheManager>();
    }
}