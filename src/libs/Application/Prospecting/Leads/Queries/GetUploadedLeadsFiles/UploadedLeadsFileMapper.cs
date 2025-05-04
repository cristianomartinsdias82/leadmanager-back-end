using Domain.Prospecting.Entities;

namespace Application.Prospecting.Leads.Queries.GetUploadedLeadsFiles;

internal static class UploadedLeadsFileMapper
{
	public static UploadedLeadsFileDto ToDto(this LeadsFile leadsFile)
		=> new(
			leadsFile.Id,
			leadsFile.FileId,
			leadsFile.UserId,
			leadsFile.FileName,
			leadsFile.CreatedAt,
			leadsFile.PathOrContainerName);
}