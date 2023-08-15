using Application.Features.Leads.Shared;
using MediatR;
using Shared.Results;

namespace Application.Features.Leads.Queries.GetLeadById;

public sealed class GetLeadByIdQueryRequest : IRequest<ApplicationResponse<LeadDto>>
{
    public required Guid Id { get; init; }
}