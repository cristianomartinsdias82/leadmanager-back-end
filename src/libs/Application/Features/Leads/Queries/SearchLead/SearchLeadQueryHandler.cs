using Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Queries.SearchLead;

internal sealed class SearchLeadQueryHandler : ApplicationRequestHandler<SearchLeadQueryRequest, bool>
{
    private readonly ILeadManagerDbContext _leadManagerDbContext;

    public SearchLeadQueryHandler(ILeadManagerDbContext leadManagerDbContext)
    {
        _leadManagerDbContext = leadManagerDbContext;
    }

    public override async Task<ApplicationResponse<bool>> Handle(SearchLeadQueryRequest request, CancellationToken cancellationToken)
    {
        var searchTerm = request.SearchTerm!.Trim();
        
        var leadExistsSearchResult = await _leadManagerDbContext
                                            .Leads
                                            .AnyAsync(ld => (!request.LeadId.HasValue || ld.Id != request.LeadId) &&
                                                            (ld.Cnpj == searchTerm || ld.RazaoSocial == searchTerm), cancellationToken);
        
        return ApplicationResponse<bool>.Create(leadExistsSearchResult);
    }
}