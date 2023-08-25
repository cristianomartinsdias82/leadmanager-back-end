using CrossCutting.Messaging.Consumers.BackendServices.Common;

namespace LeadManagerNewlyCreatedLeads.Consumer;

public class Program
{
    public async static Task Main(string[] args)
        => await ConsumerServiceHostBuilder.New(args)
                                     .WithHostedService<NewlyCreatedLeadsConsumerWorker>()
                                     .Build()
                                     .RunAsync();
}