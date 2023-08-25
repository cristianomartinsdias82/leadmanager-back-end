using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CrossCutting.Messaging.Configuration;
using CrossCutting.Messaging.RabbitMq.Configuration;

namespace CrossCutting.Messaging.Consumers.BackendServices.Common
{
    public class ConsumerServiceHostBuilder
    {
        private readonly string[] _args;
        private List<Action<IServiceCollection>> _addHostedService;

        private ConsumerServiceHostBuilder(string[] args)
        {
            _args = args;
            _addHostedService = new List<Action<IServiceCollection>>();
        }

        public static ConsumerServiceHostBuilder New(string[] args) => new(args);

        public ConsumerServiceHostBuilder WithHostedService<THostedService>() where THostedService : class, IHostedService
        {
            _addHostedService.Add(services => services.AddHostedService<THostedService>());

            return this;
        }

        public IHost Build()
            =>  Host
                    .CreateDefaultBuilder(_args)
                    .ConfigureServices(services =>
                    {
                        services
                            .AddMessageBus(services.BuildServiceProvider().GetRequiredService<IConfiguration>());

                        _addHostedService.ForEach(addHSvc => addHSvc(services));
                    })
                    .Build()
                    .UseMessageBusInitialization();
    }
}
