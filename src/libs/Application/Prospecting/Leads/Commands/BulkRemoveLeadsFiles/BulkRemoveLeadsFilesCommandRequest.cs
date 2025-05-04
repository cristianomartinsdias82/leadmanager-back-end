using MediatR;
using Shared.Results;

namespace Application.Prospecting.Leads.Commands.BulkRemoveLeadsFiles;

public sealed class BulkRemoveLeadsFilesCommandRequest : IRequest<ApplicationResponse<bool>>
{
	public required IEnumerable<RemoveLeadsFileDto> Ids { get; init; }
}
