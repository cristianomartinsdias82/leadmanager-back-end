namespace Shared.Results;

public sealed record PersistableData(byte[] DataBytes, long Length, string? ContentType, string? Name);