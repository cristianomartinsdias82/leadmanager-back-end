using MediatR;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.DownloadLeadsFile;

public sealed class DownloadLeadsFileQueryRequest : IRequest<ApplicationResponse<DownloadLeadsFileDto?>>
{
	public required Guid Id { get; init; }
}