using CrossCutting.FileStorage.Configuration;

namespace CrossCutting.FileStorage.Azure.Configuration;

public sealed class AzureStorageProviderSettings : StorageProviderSettings
{
	new public const string SectionName = nameof(AzureStorageProviderSettings);
}