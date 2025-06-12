namespace CrossCutting.FileStorage.Azure;

public sealed record AzureBlobFile : IFile
{
	public string Name { get; init; } = default!;
	public byte[] FileBytes { get; init; } = default!;
	public string? ContentType { get; init; }
	public string? Location { get; init; }
	public long ContentLength { get; init; }
}
