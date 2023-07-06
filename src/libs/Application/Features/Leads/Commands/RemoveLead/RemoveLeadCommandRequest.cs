using MediatR;
using Shared.Results;

namespace Application.Features.Leads.Commands.RemoveLead;

public sealed class RemoveLeadCommandRequest : IRequest<ApplicationResponse<RemoveLeadCommandResponse>>
{
    public required Guid Id { get; init; }
}
