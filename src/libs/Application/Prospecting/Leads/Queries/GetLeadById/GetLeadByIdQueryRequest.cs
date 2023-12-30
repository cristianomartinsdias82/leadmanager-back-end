using Domain.Prospecting.Entities;
using MediatR;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.GetLeadById;

public sealed class GetLeadByIdQueryRequest : IRequest<ApplicationResponse<LeadDto>>
{
    public required Guid Id { get; init; }
}