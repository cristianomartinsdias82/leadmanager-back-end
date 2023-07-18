﻿using MediatR;
using Shared.Results;

namespace Application.Features.Leads.Queries.SearchLead;

public sealed record SearchLeadQueryRequest(Guid? LeadId, string? SearchTerm) : IRequest<ApplicationResponse<bool>>;