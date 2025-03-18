using LeadManager.BackendServices.Consumers.Common;

namespace LeadManagerUpdatedLead.Consumer;

public class Program
{
    public async static Task Main(string[] args)
        => await ConsumerServiceHostBuilder.New(args)
                                     .WithHostedService<UpdatedLeadConsumerWorker>()
                                     .Build()
                                     .RunAsync();
}