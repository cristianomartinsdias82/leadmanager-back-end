using MediatR;
using Shared.Results;

namespace Application.Reporting.Queries.DownloadGeneratedReport;

public sealed class DownloadGeneratedReportQueryRequest : IRequest<ApplicationResponse<PersistableData?>>
{
	public required int Id { get; init; }
}