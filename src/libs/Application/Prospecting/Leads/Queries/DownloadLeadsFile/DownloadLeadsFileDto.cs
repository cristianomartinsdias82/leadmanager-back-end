namespace Application.Prospecting.Leads.Queries.DownloadLeadsFile;

public sealed record DownloadLeadsFileDto(
	byte[] FileBytes,
	string FileName,
	string? ContentType,
	long? ContentLength
);