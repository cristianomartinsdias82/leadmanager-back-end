namespace CrossCutting.FileStorage.Configuration;

public class StorageProviderSettings
{
	public const string SectionName = nameof(StorageProviderSettings);

	public string ConnectionString { get; init; } = default!;

	public string StorageFolderPath { get; init; } = default!;

	public string ContainerName { get; init; } = default!;

	public int UploadAttemptsMaxCount { get; init; } = default!;

	public int HealthCheckingTimeoutInSecs { get; init; } = default!;
}
