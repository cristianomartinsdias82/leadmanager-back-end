namespace CrossCutting.FileStorage;

public interface IFileStorageProvider
{
    Task<bool> UploadAsync(
        Stream stream,
        string name,
        bool resetStreamPositionOnRead = true,
        CancellationToken cancellationToken = default);
}
