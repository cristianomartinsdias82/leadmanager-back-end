using MediatR;
using Shared.Results;

namespace Application.Prospecting.Leads.Commands.ClearLeadsCache;

public sealed class ClearLeadsCacheCommandRequest : IRequest<ApplicationResponse<ClearLeadsCacheCommandResponse>>
{
}