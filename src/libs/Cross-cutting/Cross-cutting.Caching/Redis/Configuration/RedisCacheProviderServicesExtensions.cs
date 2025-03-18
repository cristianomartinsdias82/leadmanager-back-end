using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace CrossCutting.Caching.Redis.Configuration;

internal static class RedisCacheProviderServicesExtensions
{
    public static IServiceCollection AddRedisCacheProviderServices(
		this IServiceCollection services,
		IConfiguration configuration)
    {
        var cachingProviderSettings = configuration.GetSection(nameof(RedisCacheProviderSettings)).Get<RedisCacheProviderSettings>()!;
        services.AddSingleton(cachingProviderSettings);

		IConnectionMultiplexer redisConnMultiplexer = ConnectionMultiplexer
														.Connect($"{cachingProviderSettings.Server}:{cachingProviderSettings.PortNumber}",
																config =>
																{
																	config.AbortOnConnectFail = false;
																});
		services.AddSingleton(redisConnMultiplexer);
		services.AddStackExchangeRedisCache(options => {
			options.ConnectionMultiplexerFactory = () => Task.FromResult(redisConnMultiplexer);
		});

		services.TryAddScoped<ICacheProvider, RedisCacheProvider>();

        return services;
    }
}