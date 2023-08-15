using Application.Contracts.Persistence;
using Application.Features.Leads.Shared;
using CrossCutting.Caching;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Queries.GetLeads;

internal sealed class GetLeadsQueryHandler : ApplicationRequestHandler<GetLeadsQueryRequest, IEnumerable<LeadDto>>
{
    private readonly ILeadManagerDbContext _dbContext;
    private readonly ICacheProvider _cacheProvider;
    private const string LeadsCacheKey = "leads";
    private const int LeadsTtlInSeconds = 300;

    public GetLeadsQueryHandler(
        IMediator mediator,
        ICacheProvider cacheProvider,
        ILeadManagerDbContext dbContext) : base(mediator, default!)
    {
        _cacheProvider = cacheProvider;
        _dbContext = dbContext;
    }

    public async override Task<ApplicationResponse<IEnumerable<LeadDto>>> Handle(GetLeadsQueryRequest request, CancellationToken cancellationToken)
    {
        var cachedLeads = await _cacheProvider.GetAsync<IEnumerable<LeadDto>>(LeadsCacheKey, cancellationToken);
        if (cachedLeads is not null)
            return ApplicationResponse<IEnumerable<LeadDto>>.Create(cachedLeads);

        var leads = await _dbContext.Leads.ToListAsync(cancellationToken);

        await _cacheProvider.SetAsync(
            LeadsCacheKey,
            cachedLeads = leads.ToDtoList(),
            ttlInSeconds: LeadsTtlInSeconds,
            cancellationToken: cancellationToken);

        return ApplicationResponse<IEnumerable<LeadDto>>.Create(cachedLeads);
    }
}
