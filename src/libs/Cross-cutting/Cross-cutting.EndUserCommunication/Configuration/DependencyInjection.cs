using CrossCutting.EndUserCommunication.Sms;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.EndUserCommunication.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddEndUserCommunicationServices(this IServiceCollection services)
    {
        services.AddSmsService();

        return services;
    }

    private static IServiceCollection AddSmsService(this IServiceCollection services)
    {
        services.AddTransient<ISmsDeliveryService, DebugWriteLineBasedFakeSmsDeliveryService>();

        return services;
    }
}