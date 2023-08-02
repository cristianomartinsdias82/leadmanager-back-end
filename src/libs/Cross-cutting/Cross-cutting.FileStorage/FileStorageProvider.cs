using CrossCutting.FileStorage;

public abstract class FileStorageProvider : IFileStorageProvider
{
    public abstract Task<bool> UploadAsync(
        Stream stream,
        string name,
        bool resetStreamPositionOnRead = true,
        CancellationToken cancellationToken = default);
}
