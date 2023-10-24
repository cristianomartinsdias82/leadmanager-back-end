using Application.Prospecting.Leads.Shared;
using MediatR;
using Shared.DataPagination;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.GetLeads;

public sealed record GetLeadsQueryRequest(PaginationOptions PaginationOptions) : IRequest<ApplicationResponse<PagedList<LeadDto>>>;