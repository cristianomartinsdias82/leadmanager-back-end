using CrossCutting.FileStorage.Azure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.FileStorage.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureFileStorageServices(configuration);        

        return services;
    }
}