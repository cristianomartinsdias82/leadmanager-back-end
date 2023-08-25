using CrossCutting.Messaging.Configuration;
using CrossCutting.Messaging.RabbitMq.Configuration;

namespace LeadManagerRemovedLead.Consumer;

public class Program
{
    public async static Task Main(string[] args)
    {
        var host = Host
            .CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services
                    .AddMessageBus(services.BuildServiceProvider().GetRequiredService<IConfiguration>())
                    .AddHostedService<RemovedLeadConsumerWorker>();
            })
            .Build();

        host.UseMessageBusInitialization();

        await host.RunAsync();
    }
}