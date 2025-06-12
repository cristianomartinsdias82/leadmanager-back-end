namespace CrossCutting.FileStorage.Configuration;

public class FeatureStorageSettings
{
	public const string SectionName = nameof(FeatureStorageSettings);

	public string RootFolder { get; init; } = default!;
}