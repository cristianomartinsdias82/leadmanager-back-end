using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CrossCutting.Messaging.Configuration;
using CrossCutting.Messaging.RabbitMq.Configuration;

namespace CrossCutting.Messaging.Consumers.BackendServices.Common
{
    public static class ConsumerServiceHostFactory
    {
        public static IHost Create<THostedService>(string[] args) where THostedService : class, IHostedService
        {
            var host = Host
                .CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services
                        .AddMessageBus(services.BuildServiceProvider().GetRequiredService<IConfiguration>())
                        .AddHostedService<THostedService>();
                })
                .Build();

            host.UseMessageBusInitialization();

            return host;
        }
    }
}
