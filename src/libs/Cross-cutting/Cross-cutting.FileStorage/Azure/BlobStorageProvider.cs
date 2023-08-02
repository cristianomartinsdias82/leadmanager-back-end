using Azure.Storage.Blobs;
using CrossCutting.FileStorage.Azure.Configuration;
using Polly;
using Shared.FrameworkExtensions;

namespace CrossCutting.FileStorage.Azure;

internal sealed class BlobStorageProvider : FileStorageProvider
{
    private readonly AzureStorageProviderSettings _settings;

    public BlobStorageProvider(AzureStorageProviderSettings settings)
    {
        _settings = settings;
    }

    public override async Task<bool> UploadAsync(
        Stream stream,
        string name,
        bool resetStreamPositionOnRead,
        CancellationToken cancellationToken = default)
    {
        var getStreamBytesResult = await stream.TryGetBytesAsync(
            resetStreamPositionOnRead,
            cancellationToken);

        if (!getStreamBytesResult.Successful)
            return false;

        var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);

        return await Policy.Handle<Exception>()
                .WaitAndRetryAsync(_settings.MaxUploadRetries, count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)))
                .ExecuteAsync(async () =>
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(name);

            await blobClient.UploadAsync(new BinaryData(getStreamBytesResult.ByteArray), overwrite: true, cancellationToken);

            return true;
        });
    }
}