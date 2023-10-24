using Application.Core.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.SearchLead;

internal sealed class SearchLeadQueryRequestHandler : ApplicationRequestHandler<SearchLeadQueryRequest, bool>
{
    private readonly ILeadManagerDbContext _leadManagerDbContext;

    public SearchLeadQueryRequestHandler(
        IMediator mediator,
        ILeadManagerDbContext leadManagerDbContext) : base(mediator, default!)
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