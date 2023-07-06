using Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Queries.SearchByCnpjOrRazaoSocial;

internal sealed class SearchByCnpjOrRazaoSocialQueryHandler : ApplicationRequestHandler<SearchByCnpjOrRazaoSocialQueryRequest, bool>
{
    private readonly ILeadManagerDbContext _leadManagerDbContext;

    public SearchByCnpjOrRazaoSocialQueryHandler(ILeadManagerDbContext leadManagerDbContext)
    {
        _leadManagerDbContext = leadManagerDbContext;
    }

    public override async Task<ApplicationResponse<bool>> Handle(SearchByCnpjOrRazaoSocialQueryRequest request, CancellationToken cancellationToken)
    {
        var leadExistsSearchResult = await _leadManagerDbContext.Leads.AnyAsync(ld => ld.Cnpj == request.CnpjOrRazaoSocial || ld.RazaoSocial == request.CnpjOrRazaoSocial ,cancellationToken);
        
        return ApplicationResponse<bool>.Create(leadExistsSearchResult);
    }
}