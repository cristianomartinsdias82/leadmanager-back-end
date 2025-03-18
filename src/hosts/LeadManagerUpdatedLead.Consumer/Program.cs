using CrossCutting.Messaging.Consumers.BackendServices.Common;

namespace LeadManagerUpdatedLead.Consumer;

public class Program
{
    public async static Task Main(string[] args)
        => await ConsumerServiceHostBuilder.New(args)
                                     .WithHostedService<UpdatedLeadConsumerWorker>()
                                     .Build()
                                     .RunAsync();
}