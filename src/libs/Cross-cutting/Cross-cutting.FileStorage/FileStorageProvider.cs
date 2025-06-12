namespace CrossCutting.FileStorage;

public abstract class FileStorageProvider : IFileStorageProvider
{
	public abstract Task<bool> UploadAsync(
        ReadOnlyMemory<byte> bytes,
        string blobName,
		string? blobPath = default,
        CancellationToken cancellationToken = default);

	public abstract Task<IFile?> DownloadAsync(
		string blobName,
		string? blobPath = default,
		string? containerName = default,
		CancellationToken cancellationToken = default);

	public abstract Task BatchRemoveAsync(
		IEnumerable<string> blobNames,
		string? containerName = default,
		CancellationToken cancellationToken = default);
}
