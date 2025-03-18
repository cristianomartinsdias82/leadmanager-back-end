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
        => services.AddRedisCacheProviderServices(configuration, hostEnvironment);

    public static TracerProviderBuilder AddCachingTracing(
        this TracerProviderBuilder tracerProviderBuilder)
        => tracerProviderBuilder.AddRedisInstrumentation();
}