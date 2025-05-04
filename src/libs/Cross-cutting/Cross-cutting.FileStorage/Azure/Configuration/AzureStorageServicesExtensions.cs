using CrossCutting.FileStorage.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CrossCutting.FileStorage.Azure.Configuration;

internal static class AzureStorageServicesExtensions
{
    public static IServiceCollection AddAzureFileStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        var storageProviderSettings = configuration
                                .GetSection(AzureStorageProviderSettings.SectionName)
                                .Get<AzureStorageProviderSettings>()!;

        services.AddSingleton<StorageProviderSettings>(_ => storageProviderSettings);

        services.TryAddSingleton<IFileStorageProvider, BlobStorageProvider>();

        return services;
    }
}