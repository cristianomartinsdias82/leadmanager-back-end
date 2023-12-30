using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CrossCutting.Caching.Redis.Configuration;

internal static class RedisCacheProviderServicesExtensions
{
    public static IServiceCollection AddRedisCacheProviderServices(this IServiceCollection services, IConfiguration configuration)
    {
        var cachingProviderSettings = configuration.GetSection(nameof(RedisCacheProviderSettings)).Get<RedisCacheProviderSettings>()!;
        services.AddSingleton(cachingProviderSettings);

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = $"{cachingProviderSettings.Server}:{cachingProviderSettings.PortNumber}";
        });
        services.TryAddScoped<ICacheProvider, RedisCacheProvider>();

        return services;
    }
}