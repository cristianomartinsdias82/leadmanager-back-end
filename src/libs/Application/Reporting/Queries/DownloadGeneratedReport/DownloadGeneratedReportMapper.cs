using CrossCutting.FileStorage;
using Shared.Results;

namespace Application.Reporting.Queries.DownloadGeneratedReport;

internal static class DownloadGeneratedReportMapper
{
	public static PersistableData? ToDto(this IFile? leadsFile)
		=> leadsFile is null
			? default
			: new(
				leadsFile.FileBytes,
				leadsFile.ContentLength,
				leadsFile.ContentType,
				leadsFile.Name
			);
}