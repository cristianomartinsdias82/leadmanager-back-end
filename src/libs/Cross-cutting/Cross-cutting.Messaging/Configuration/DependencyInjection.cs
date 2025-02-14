using CrossCutting.Messaging.RabbitMq.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace CrossCutting.Messaging.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var messageChannelsSettings = configuration.GetSection(nameof(MessageChannelSettings)).Get<MessageChannelSettings>()!;
        services.AddSingleton(messageChannelsSettings);
        services.AddRabbitMqMessageBusServices(configuration);

        return services;
    }

	public static TracerProviderBuilder AddMessageBusTracing(
		this TracerProviderBuilder tracerProviderBuilder,
		IServiceCollection services,
		IConfiguration configuration)
		=> tracerProviderBuilder.AddRabbitMqMessageBusTracing(services, configuration);
}