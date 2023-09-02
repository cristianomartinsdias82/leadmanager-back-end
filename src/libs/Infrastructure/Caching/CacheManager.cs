using Application.Contracts.Caching;
using Application.Contracts.Caching.Policies;
using Application.Contracts.Persistence;
using Application.Features.Leads.Shared;
using Core.Entities;
using CrossCutting.Caching;
using CrossCutting.MessageContracts;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Shared.DataPagination;
using Shared.FrameworkExtensions;

namespace Infrastructure.Caching;

internal sealed class CacheManager : ICachingManagement
{
    private readonly ICacheProvider _cacheProvider;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly CachingPolicy _leadsCachingPolicy;

    public CacheManager(
        ILeadManagerDbContext dbContext,
        ICacheProvider cacheProvider,
        CachingPoliciesSettings cachingPoliciesSettings)
    {
        _dbContext = dbContext;
        _cacheProvider = cacheProvider;
        _leadsCachingPolicy = cachingPoliciesSettings.LeadsPolicy;
    }

    public async Task<PagedList<LeadDto>> GetLeadsAsync(
        PaginationOptions paginationOptions,
        CancellationToken cancellationToken = default)
    {
        var cachedLeads = await _cacheProvider.GetAsync<IEnumerable<LeadData>>(
            _leadsCachingPolicy.CacheKey,
            cancellationToken);

        if (cachedLeads is not null)
            return cachedLeads.ToSortedPagedList(
                paginationOptions.SortColumn ?? nameof(Lead.RazaoSocial),
                paginationOptions.SortDirection,
                paginationOptions,
                ld => ld.AsDtoList());

        var leads = await _dbContext.Leads.ToListAsync(cancellationToken);

        if (leads.Count.Equals(0))
            return PagedList<LeadDto>.Empty();

        await _cacheProvider.SetAsync(
            _leadsCachingPolicy.CacheKey,
            cachedLeads = leads.Select(ld => ld.AsMessageContract()),
            ttlInSeconds: _leadsCachingPolicy.TtlInSeconds,
            cancellationToken: cancellationToken);

        return leads.ToSortedPagedList(
            paginationOptions.SortColumn ?? nameof(Lead.RazaoSocial),
            paginationOptions.SortDirection,
            paginationOptions,
            ld => ld.AsDtoList());
    }

    public async Task AddLeadEntryAsync(LeadDto lead, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lead);

        var cachedLeads = await _cacheProvider
                                    .GetAsync<IEnumerable<LeadData>>(
                                        _leadsCachingPolicy.CacheKey,
                                        cancellationToken);
        var leads = cachedLeads?.ToList() ?? new();
        leads.Add(lead.AsMessageContract());

        await _cacheProvider.SetAsync<IEnumerable<LeadData>>(
                _leadsCachingPolicy.CacheKey,
                leads,
                _leadsCachingPolicy.TtlInSeconds,
                cancellationToken);
    }

    public async Task AddLeadEntriesAsync(IList<LeadDto> leads, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(leads);

        var cachedLeads = await _cacheProvider
                                    .GetAsync<IEnumerable<LeadData>>(
                                        _leadsCachingPolicy.CacheKey,
                                        cancellationToken);
        var existingLeads = cachedLeads?.ToList() ?? new();
        existingLeads.AddRange(leads.AsMessageContractList());

        await _cacheProvider.SetAsync<IEnumerable<LeadData>>(
                _leadsCachingPolicy.CacheKey,
                existingLeads,
                _leadsCachingPolicy.TtlInSeconds,
                cancellationToken);
    }

    public async Task ClearLeadEntriesAsync(CancellationToken cancellationToken = default)
        => await _cacheProvider.RemoveAsync(_leadsCachingPolicy.CacheKey, cancellationToken);

    public async Task RemoveLeadEntryAsync(LeadDto lead, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lead);

        var cachedLeads = await _cacheProvider
                                    .GetAsync<IEnumerable<LeadData>>(
                                        _leadsCachingPolicy.CacheKey,
                                        cancellationToken);
        if (!cachedLeads?.Any() ?? false)
            return;

        var leads = cachedLeads!.ToList();
        var leadToRemove = leads.FirstOrDefault(ld => ld.Id == lead.Id);
        if (leadToRemove.IsNull)
            return;

        leads.Remove(leadToRemove);
        await _cacheProvider.SetAsync<IEnumerable<LeadData>>(
                _leadsCachingPolicy.CacheKey,
                leads,
                _leadsCachingPolicy.TtlInSeconds,
                cancellationToken);
    }

    public async Task UpdateLeadEntryAsync(LeadDto lead, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lead);

        var cachedLeads = await _cacheProvider.GetAsync<IEnumerable<LeadData>>(
            _leadsCachingPolicy.CacheKey,
            cancellationToken);

        if (!cachedLeads?.Any() ?? false)
            return;

        var leads = cachedLeads!.ToList();
        var outdatedLead = leads.FirstOrDefault(ld => ld.Id == lead.Id);
        if (outdatedLead.IsNull)
            return;

        leads.Remove(outdatedLead);
        leads.Add(lead.AsMessageContract());

        await _cacheProvider.SetAsync<IEnumerable<LeadData>>(
                _leadsCachingPolicy.CacheKey,
                leads,
                _leadsCachingPolicy.TtlInSeconds,
                cancellationToken);
    }
}
