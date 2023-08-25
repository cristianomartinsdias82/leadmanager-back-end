using CrossCutting.Messaging.Consumers.BackendServices.Common;

namespace LeadManagerRemovedLead.Consumer;

public class Program
{
    public async static Task Main(string[] args)
        => await ConsumerServiceHostBuilder.New(args)
                                     .WithHostedService<RemovedLeadConsumerWorker>()
                                     .Build()
                                     .RunAsync();
}