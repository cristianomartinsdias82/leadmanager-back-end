using Application.Features.Leads.Shared;
using MediatR;
using Shared.DataPagination;
using Shared.Results;

namespace Application.Features.Leads.Queries.GetLeads;

public sealed record GetLeadsQueryRequest(PaginationOptions PaginationOptions) : IRequest<ApplicationResponse<PagedList<LeadDto>>>;