using CrossCutting.EndUserCommunication.Sms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.EndUserCommunication;

public static class DependencyInjection
{
    public static IServiceCollection AddEndUserCommunicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSmsService(configuration);

        return services;
    }

    private static IServiceCollection AddSmsService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ISmsDeliveryService, DebugWriteLineBasedFakeSmsDeliveryService>();

        return services;
    }
}