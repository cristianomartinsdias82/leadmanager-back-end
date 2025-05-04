using CrossCutting.FileStorage;

namespace Application.Prospecting.Leads.Queries.DownloadLeadsFile;

internal static class DownloadLeadsFileMapper
{
	public static DownloadLeadsFileDto? ToDto(this IFile? leadsFile)
		=> leadsFile is null
			? default
			: new(
				leadsFile.FileBytes,
				leadsFile.Name,
				leadsFile.ContentType,
				leadsFile.ContentLength);
}