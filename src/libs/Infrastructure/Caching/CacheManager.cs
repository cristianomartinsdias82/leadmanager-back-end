using Application.Contracts.Caching;
using Application.Contracts.Caching.Policies;
using Application.Contracts.Persistence;
using Application.Features.Leads.Shared;
using CrossCutting.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Caching;

internal sealed class CacheManager : ICachingManagement
{
    private readonly ICacheProvider _cacheProvider;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly CachingPolicy _leadsCachingPolicy;
    private readonly ILogger<CacheManager> _logger;

    public CacheManager(
        ILeadManagerDbContext dbContext,
        ICacheProvider cacheProvider,
        CachingPoliciesSettings cachingPoliciesSettings,
        ILogger<CacheManager> logger)
    {
        _dbContext = dbContext;
        _cacheProvider = cacheProvider;
        _leadsCachingPolicy = cachingPoliciesSettings.LeadsPolicy;
        _logger = logger;
    }

    public async Task<IEnumerable<LeadDto>> GetLeadsAsync(CancellationToken cancellationToken = default)
    {
        var cachedLeads = await _cacheProvider.GetAsync<IEnumerable<LeadDto>>(
            _leadsCachingPolicy.CacheKey,
            cancellationToken);

        if (cachedLeads is not null)
            return cachedLeads;

        var leads = await _dbContext.Leads.ToListAsync(cancellationToken);

        await _cacheProvider.SetAsync(
            _leadsCachingPolicy.CacheKey,
            cachedLeads = leads.ToDtoList(),
            ttlInSeconds: _leadsCachingPolicy.TtlInSeconds,
            cancellationToken: cancellationToken);

        return cachedLeads;
    }

    public async Task AddLeadEntryAsync(LeadDto lead, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lead);

        var cachedLeads = await GetLeadsAsync(cancellationToken);
        var leads = cachedLeads.ToList();
        leads.Add(lead);

        await _cacheProvider.SetAsync<IEnumerable<LeadDto>>(
                _leadsCachingPolicy.CacheKey,
                leads,
                _leadsCachingPolicy.TtlInSeconds,
                cancellationToken);
    }

    public async Task AddLeadEntriesAsync(List<LeadDto> leads, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(leads);

        var cachedLeads = await GetLeadsAsync(cancellationToken);
        var existingLeads = cachedLeads.ToList();
        existingLeads.AddRange(leads);

        await _cacheProvider.SetAsync<IEnumerable<LeadDto>>(
                _leadsCachingPolicy.CacheKey,
                existingLeads,
                _leadsCachingPolicy.TtlInSeconds,
                cancellationToken);
    }

    public async Task ClearLeadEntriesAsync(CancellationToken cancellationToken = default)
    {
        await _cacheProvider.RemoveAsync(_leadsCachingPolicy.CacheKey, cancellationToken);
    }

    public async Task RemoveLeadEntryAsync(LeadDto lead, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lead);

        var cachedLeads = await GetLeadsAsync(cancellationToken);
        var leads = cachedLeads.ToList();
        var leadToRemove = leads.FirstOrDefault(ld => ld.Id == lead.Id);
        if (leadToRemove is null)
            return;

        leads.Remove(leadToRemove);
        await _cacheProvider.SetAsync<IEnumerable<LeadDto>>(
                _leadsCachingPolicy.CacheKey,
                leads,
                _leadsCachingPolicy.TtlInSeconds,
                cancellationToken);
    }

    public async Task UpdateLeadEntryAsync(LeadDto lead, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lead);

        var cachedLeads = await GetLeadsAsync(cancellationToken);
        var leads = cachedLeads.ToList();
        var outdatedLead = leads.FirstOrDefault(ld => ld.Id == lead.Id);

        if (outdatedLead is null)
            return;

        leads.Remove(outdatedLead);
        leads.Add(lead);

        await _cacheProvider.SetAsync<IEnumerable<LeadDto>>(
                _leadsCachingPolicy.CacheKey,
                leads,
                _leadsCachingPolicy.TtlInSeconds,
                cancellationToken);
    }
}
