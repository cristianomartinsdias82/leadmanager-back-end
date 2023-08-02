using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CrossCutting.FileStorage.Azure.Configuration;

namespace CrossCutting.FileStorage.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureStorageServices(configuration);        

        return services;
    }
}