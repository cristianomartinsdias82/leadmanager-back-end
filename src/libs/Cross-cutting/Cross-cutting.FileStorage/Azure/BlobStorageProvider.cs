using Azure.Storage.Blobs;
using CrossCutting.FileStorage.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using System.Text.Json;

namespace CrossCutting.FileStorage.Azure;

internal sealed class BlobStorageProvider : FileStorageProvider
{
	private readonly StorageProviderSettings _storageProvidersettings;
	private readonly ILogger<BlobStorageProvider> _logger;

	public BlobStorageProvider(
		StorageProviderSettings storageProviderSettings,
		ILogger<BlobStorageProvider> logger)
	{
		_storageProvidersettings = storageProviderSettings;
		_logger = logger;
	}

	public override async Task<bool> UploadAsync(
		ReadOnlyMemory<byte> bytes,
		string blobName,
		bool resetStreamPositionOnRead,
		CancellationToken cancellationToken = default)
	{
		var serviceClient = new BlobServiceClient(_storageProvidersettings.ConnectionString);

		return await Policy.Handle<Exception>()
				.WaitAndRetryAsync(_storageProvidersettings.UploadAttemptsMaxCount, count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)))
				.ExecuteAsync(async () =>
		{
			var containerClient = serviceClient.GetBlobContainerClient(_storageProvidersettings.ContainerName);

			await containerClient.CreateIfNotExistsAsync();

			var blobClient = containerClient.GetBlobClient(blobName);

			await blobClient.UploadAsync(
								BinaryData.FromBytes(bytes),
								overwrite: true,
								cancellationToken);

			return true;
		});
	}

	public override async Task<IFile?> DownloadAsync(
		string blobName,
		string? containerName,
		CancellationToken cancellationToken = default)
		=> await Policy.Handle<Exception>()
				.WaitAndRetryAsync(_storageProvidersettings.UploadAttemptsMaxCount, count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)))
				.ExecuteAsync(async () =>
				{
					var serviceClient = new BlobServiceClient(_storageProvidersettings.ConnectionString);

					var containerClient = serviceClient.GetBlobContainerClient(containerName ?? _storageProvidersettings.ContainerName);

					var blobClient = containerClient.GetBlobClient(blobName);

					using var ms = new MemoryStream();

					using var downloadResponse = await blobClient.DownloadToAsync(ms, cancellationToken);

					if (downloadResponse.IsError)
					{
						_logger.LogError("Error while attempting to download file {file} from container {container}. Reason: {reason}. Http headers: {httpHeaders}",
										blobName,
										containerName,
										downloadResponse.ReasonPhrase,
										JsonSerializer.Serialize(downloadResponse.Headers.ToDictionary(x => x.Name, x => x.Value)));

						return default;
					}

					return new AzureBlobFile
					{
						Name = blobName,
						FileBytes = ms.ToArray(),
						ContentType = downloadResponse.Headers.ContentType,
						ContentLength = ms.Length
					};
				});

	public async override Task BatchRemoveAsync(
		IEnumerable<string> blobNames,
		string? containerName,
		CancellationToken cancellationToken = default)
		=> await Policy.Handle<Exception>()
				.WaitAndRetryAsync(_storageProvidersettings.UploadAttemptsMaxCount, count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)))
				.ExecuteAsync(async () =>
				{
					var serviceClient = new BlobServiceClient(_storageProvidersettings.ConnectionString);

					var containerClient = serviceClient.GetBlobContainerClient(containerName ?? _storageProvidersettings.ContainerName);

					await Parallel.ForEachAsync(blobNames, async (blobName, ct) => await containerClient.DeleteBlobIfExistsAsync(blobName, cancellationToken: ct));
				});
}