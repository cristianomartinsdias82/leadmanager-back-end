using MediatR;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.SearchLead;

public sealed record SearchLeadQueryRequest(Guid? LeadId, string? SearchTerm) : IRequest<ApplicationResponse<bool>>;