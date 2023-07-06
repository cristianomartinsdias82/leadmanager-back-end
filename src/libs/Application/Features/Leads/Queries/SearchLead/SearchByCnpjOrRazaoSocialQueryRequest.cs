using MediatR;
using Shared.Results;

namespace Application.Features.Leads.Queries.SearchByCnpjOrRazaoSocial;

public sealed record SearchByCnpjOrRazaoSocialQueryRequest(string CnpjOrRazaoSocial) : IRequest<ApplicationResponse<bool>>;