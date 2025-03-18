using CrossCutting.Messaging.RabbitMq.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry.Trace;

namespace CrossCutting.Messaging.RabbitMq.Configuration;

internal static class RabbitMqServiceExtensions
{
    public static IServiceCollection AddRabbitMqMessageBusServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rabbitMqConnectionSettings = configuration.GetSection(nameof(RabbitMqConnectionSettings)).Get<RabbitMqConnectionSettings>()!;
        services.AddSingleton(rabbitMqConnectionSettings);

        services.TryAddSingleton<IRabbitMqChannelFactory, RabbitMqChannelFactory>();
        services.TryAddSingleton<IMessageDispatching, RabbitMqMessageDispatcher>();
        services.TryAddTransient<IMessageConsumption, RabbitMqMessageConsumer>();

        return services;
    }

	public static TracerProviderBuilder AddRabbitMqMessageBusTracing(
		this TracerProviderBuilder tracerProviderBuilder,
		IServiceCollection services,
		IConfiguration configuration)
		=> tracerProviderBuilder.AddSource(RabbitMqDiagnostics.ActivitySourceName);
}
