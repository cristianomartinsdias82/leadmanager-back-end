namespace CrossCutting.FileStorage;

public interface IFileStorageProvider
{
    Task<bool> UploadAsync(
        ReadOnlyMemory<byte> bytes,
        string blobName,
		string? blobPath = default,
        CancellationToken cancellationToken = default);

    Task<IFile?> DownloadAsync(
		string blobName,
		string? blobPath = default,
		string? containerName = default,
		CancellationToken cancellationToken = default);

	Task BatchRemoveAsync(
			IEnumerable<string> blobNames,
			string? containerName = default,
			CancellationToken cancellationToken = default);
}
