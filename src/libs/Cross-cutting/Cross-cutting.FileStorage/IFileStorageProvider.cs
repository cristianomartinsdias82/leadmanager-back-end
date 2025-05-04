namespace CrossCutting.FileStorage;

public interface IFileStorageProvider
{
    Task<bool> UploadAsync(
        ReadOnlyMemory<byte> bytes,
        string blobName,
        bool resetStreamPositionOnRead = true,
        CancellationToken cancellationToken = default);

    Task<IFile?> DownloadAsync(
		string blobName,
		string? containerName,
		CancellationToken cancellationToken = default);

	Task BatchRemoveAsync(
			IEnumerable<string> blobNames,
			string? containerName,
			CancellationToken cancellationToken = default);
}
