using MediatR;
using Shared.Results;

namespace Application.Prospecting.Leads.Commands.RemoveLead;

public sealed class RemoveLeadCommandRequest : IRequest<ApplicationResponse<RemoveLeadCommandResponse>>
{
    public required Guid Id { get; init; }
    public byte[]? Revision { get; set; }
}
