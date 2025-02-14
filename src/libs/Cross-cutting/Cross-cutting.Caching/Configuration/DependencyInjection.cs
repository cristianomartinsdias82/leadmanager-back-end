using CrossCutting.Caching.Redis.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace CrossCutting.Caching.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
        => services.AddRedisCacheProviderServices(configuration);

    public static TracerProviderBuilder AddCachingTracing(
        this TracerProviderBuilder tracerProviderBuilder,
		IServiceCollection services,
        IConfiguration configuration)
        => tracerProviderBuilder.AddRedisInstrumentation();
}