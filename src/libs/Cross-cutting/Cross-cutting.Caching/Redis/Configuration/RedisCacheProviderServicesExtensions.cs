using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace CrossCutting.Caching.Redis.Configuration;

internal static class RedisCacheProviderServicesExtensions
{
    public static IServiceCollection AddRedisCacheProviderServices(
		this IServiceCollection services,
		IConfiguration configuration,
		IHostEnvironment hostEnvironment)
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

		//Presentation layer cache
		services.AddStackExchangeRedisOutputCache(options =>
		{
			options.InstanceName = $"{hostEnvironment.EnvironmentName}_PresentationCacheLayer";
			options.ConnectionMultiplexerFactory = () => Task.FromResult(redisConnMultiplexer);
		});

		//Application layer cache
		services.AddStackExchangeRedisCache(options => {
			options.InstanceName = $"{hostEnvironment.EnvironmentName}_ApplicationCacheLayer";
			options.ConnectionMultiplexerFactory = () => Task.FromResult(redisConnMultiplexer);
		});

		services.TryAddScoped<ICacheProvider, RedisCacheProvider>();

        return services;
    }
}