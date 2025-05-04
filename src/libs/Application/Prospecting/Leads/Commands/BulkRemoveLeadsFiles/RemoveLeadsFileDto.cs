namespace Application.Prospecting.Leads.Commands.BulkRemoveLeadsFiles;

public sealed record RemoveLeadsFileDto(
	Guid Id,
	string FileId);