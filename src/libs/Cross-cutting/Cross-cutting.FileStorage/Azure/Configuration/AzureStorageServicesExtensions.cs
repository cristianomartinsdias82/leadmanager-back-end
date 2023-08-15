using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CrossCutting.FileStorage.Azure.Configuration;

internal static class AzureStorageServicesExtensions
{
    public static IServiceCollection AddAzureStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        var storageSettings = configuration.GetSection(nameof(AzureStorageProviderSettings)).Get<AzureStorageProviderSettings>()!;
        services.AddSingleton(storageSettings);

        services.TryAddSingleton<IFileStorageProvider, BlobStorageProvider>();

        return services;
    }
}