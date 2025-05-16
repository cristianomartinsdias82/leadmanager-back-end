using Shared.Generators;

namespace Domain.Prospecting.Entities;

public sealed class LeadsFile
{
	private LeadsFile() { }

	public Guid Id { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public string UserId { get; private set; } = default!;
	public string FileId { get; private set; } = default!;
	public string FileName { get; private set; } = default!;
	public string? Description { get; private set; }
	public string? PathOrContainerName { get; private set; }

	public static LeadsFile Create(
		DateTimeOffset createdAt,
		string userId,
		string fileId,
		string fileName,
		string? pathOrContainerName,
		string? description)
		=> new()
		{
			Id = IdGenerator.NextId(),
			CreatedAt = createdAt,
			UserId = userId,
			FileId = fileId,
			FileName = fileName,
			PathOrContainerName = pathOrContainerName,
			Description = description
		};

	public static string CreateFileId(string fileExtension)
		=> $"{Guid.NewGuid()}{fileExtension}";
}
