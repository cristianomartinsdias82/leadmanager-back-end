using CrossCutting.Messaging.RabbitMq.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.Messaging.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
        => services.AddRabbitMqMessageBusServices(configuration);
}