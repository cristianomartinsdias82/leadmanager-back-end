namespace CrossCutting.FileStorage.Azure.Configuration;

public sealed record AzureStorageProviderSettings
(
    string ConnectionString,
    string StorageFolderPath,
    string ContainerName,
    int UploadAttemptsMaxCount
);