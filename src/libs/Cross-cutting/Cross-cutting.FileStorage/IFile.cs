namespace CrossCutting.FileStorage;

public interface IFile
{
	string Name { get; init; }
	byte[] FileBytes { get; init; }
	string? ContentType { get; init; }
	string? Location { get; init; }
	long? ContentLength { get; init; }
}
