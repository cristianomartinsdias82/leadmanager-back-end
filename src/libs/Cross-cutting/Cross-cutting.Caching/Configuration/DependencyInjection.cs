using CrossCutting.Caching.Hybrid;
using CrossCutting.Caching.Hybrid.Configuration;
using CrossCutting.Caching.Redis.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;

namespace CrossCutting.Caching.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment)
    {
		var cachingProviderSettings = configuration.GetSection(nameof(HybridCacheProviderSettings)).Get<HybridCacheProviderSettings>()!;
		services.AddSingleton(cachingProviderSettings);
		services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = cachingProviderSettings.MaximumPayloadBytes;
        });
		services.AddScoped<ICacheProvider, HybridCacheProvider>();
		services.AddRedisCacheProviderServices(configuration, hostEnvironment);

		return services;
    }

    public static TracerProviderBuilder AddCachingTracing(
        this TracerProviderBuilder tracerProviderBuilder)
        => tracerProviderBuilder.AddRedisInstrumentation();
}