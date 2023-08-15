using Application.Features.Leads.Shared;
using MediatR;
using Shared;
using Shared.Results;

namespace Application.Features.Leads.Queries.GetLeads;

public sealed record GetLeadsQueryRequest(PaginationOptions PaginationOptions) : IRequest<ApplicationResponse<IEnumerable<LeadDto>>>;