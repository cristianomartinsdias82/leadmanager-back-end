namespace Application.Prospecting.Leads.Queries.GetUploadedLeadsFiles;

public sealed record UploadedLeadsFileDto(
	Guid Id,
	string FileId,
	string UserId,
	string FileName,
	DateTimeOffset CreatedAt,
	string? PathOrContainerName
);