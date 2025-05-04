using CrossCutting.FileStorage;

public abstract class FileStorageProvider : IFileStorageProvider
{
	public abstract Task<bool> UploadAsync(
        ReadOnlyMemory<byte> bytes,
        string blobName,
        bool resetStreamPositionOnRead = true,
        CancellationToken cancellationToken = default);

	public abstract Task<IFile?> DownloadAsync(
		string blobName,
		string? containerName,
		CancellationToken cancellationToken = default);

	public abstract Task BatchRemoveAsync(
		IEnumerable<string> blobNames,
		string? containerName,
		CancellationToken cancellationToken = default);
}
