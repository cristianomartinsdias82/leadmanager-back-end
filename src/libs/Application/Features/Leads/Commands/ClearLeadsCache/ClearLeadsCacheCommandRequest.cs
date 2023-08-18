using MediatR;
using Shared.Results;

namespace Application.Features.Leads.Commands.ClearLeadsCache;

public sealed class ClearLeadsCacheCommandRequest : IRequest<ApplicationResponse<ClearLeadsCacheCommandResponse>>
{
}