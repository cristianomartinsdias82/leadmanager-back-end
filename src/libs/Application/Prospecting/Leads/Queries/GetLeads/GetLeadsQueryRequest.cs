﻿using Domain.Prospecting.Entities;
using MediatR;
using Shared.DataQuerying;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.GetLeads;

public sealed record GetLeadsQueryRequest(
						string? Search,
						PaginationOptions PaginationOptions) : IRequest<ApplicationResponse<PagedList<LeadDto>>>;