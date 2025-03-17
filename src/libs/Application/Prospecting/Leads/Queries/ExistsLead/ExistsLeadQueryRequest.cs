using MediatR;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.ExistsLead;

public sealed record ExistsLeadQueryRequest(Guid? LeadId, string? SearchTerm) : IRequest<ApplicationResponse<bool>>;