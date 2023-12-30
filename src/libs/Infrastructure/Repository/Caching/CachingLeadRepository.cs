using Application.Core.Contracts.Repository.Prospecting;
using Application.Core.Contracts.Repository.Caching;
using Application.Core.Contracts.Repository.UnitOfWork;
using Application.Prospecting.Leads.Shared;
using CrossCutting.Caching;
using CrossCutting.MessageContracts;
using Domain.Prospecting.Entities;
using Shared.DataPagination;
using Shared.FrameworkExtensions;
using System.Linq.Expressions;

namespace Infrastructure.Repository.Caching;

internal sealed class CachingLeadRepository : ILeadRepository, ICachingLeadRepository
{
    private readonly ILeadRepository _leadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheProvider _cacheProvider;
    private readonly LeadsCachingPolicy _leadsCachingPolicy;

    public CachingLeadRepository(
        ILeadRepository leadRepository,
        IUnitOfWork unitOfWork,
        ICacheProvider cacheProvider,
        LeadsCachingPolicy leadsCachingPolicy)
    {
        _leadRepository = leadRepository;
        _unitOfWork = unitOfWork;
        _cacheProvider = cacheProvider;
        _leadsCachingPolicy = leadsCachingPolicy;
    }

    public async Task<PagedList<Lead>> GetAsync(
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
                ld => ld.MapToEntity());

        var leads = await _leadRepository.GetAsync(paginationOptions, cancellationToken);
        if (leads.ItemCount.Equals(0))
            return PagedList<Lead>.Empty();

        await _cacheProvider.SetAsync(
            _leadsCachingPolicy.CacheKey,
            cachedLeads = leads.Items.Select(ld => ld.MapToMessageContract()),
            ttlInSeconds: _leadsCachingPolicy.TtlInSeconds,
            cancellationToken: cancellationToken);

        return leads.Items.ToSortedPagedList(
            paginationOptions.SortColumn ?? nameof(Lead.RazaoSocial),
            paginationOptions.SortDirection,
            paginationOptions,
            leads => leads);
    }

    public async Task<Lead?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cachedLeads = await _cacheProvider.GetAsync<IEnumerable<LeadData>>(
            _leadsCachingPolicy.CacheKey,
            cancellationToken);

        if (!cachedLeads?.Any() ?? false)
        {
            var lead = cachedLeads!.SingleOrDefault(ld => ld.Id == id);
            if (!lead.IsNull)
                return lead.MapToEntity();
        }

        return await _leadRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task AddAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        await _leadRepository.AddAsync(lead, cancellationToken);
        var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken);
        if (rowsAffected.Equals(0))
            return;

        var cachedLeads = await _cacheProvider
                                    .GetAsync<IEnumerable<LeadData>>(
                                        _leadsCachingPolicy.CacheKey,
                                        cancellationToken);
        var leads = cachedLeads?.ToList() ?? new();
        leads.Add(lead.MapToMessageContract());

        await _cacheProvider.SetAsync(
                _leadsCachingPolicy.CacheKey,
                leads,
                _leadsCachingPolicy.TtlInSeconds,
                cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Lead> leads, CancellationToken cancellationToken = default)
    {
        await _leadRepository.AddRangeAsync(leads, cancellationToken);
        var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken);
        if (rowsAffected.Equals(0))
            return;

        var cachedLeads = await _cacheProvider
                                    .GetAsync<IEnumerable<LeadData>>(
                                        _leadsCachingPolicy.CacheKey,
                                        cancellationToken);
        var existingLeads = cachedLeads?.ToList() ?? new();
        existingLeads.AddRange(leads.MapToMessageContractList());

        await _cacheProvider.SetAsync<IEnumerable<LeadData>>(
                _leadsCachingPolicy.CacheKey,
                existingLeads,
                _leadsCachingPolicy.TtlInSeconds,
                cancellationToken);
    }

    public async Task RemoveAsync(Lead lead, byte[] rowVersion, CancellationToken cancellationToken = default)
    {
        await _leadRepository.RemoveAsync(lead, rowVersion, cancellationToken);
        var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken);
        if (rowsAffected.Equals(0))
            return;

        var cachedLeads = await _cacheProvider
                                    .GetAsync<IEnumerable<LeadData>>(
                                        _leadsCachingPolicy.CacheKey,
                                        cancellationToken);
        if (!cachedLeads?.Any() ?? false)
            return;

        var leads = cachedLeads!.ToList();
        var leadToRemove = leads.SingleOrDefault(ld => ld.Id == lead.Id);
        if (leadToRemove.IsNull)
            return;

        leads.Remove(leadToRemove);
        await _cacheProvider.SetAsync<IEnumerable<LeadData>>(
                _leadsCachingPolicy.CacheKey,
                leads,
                _leadsCachingPolicy.TtlInSeconds,
                cancellationToken);
    }

    public async Task UpdateAsync(Lead lead, byte[] rowVersion, CancellationToken cancellationToken = default)
    {
        await _leadRepository.UpdateAsync(lead, rowVersion, cancellationToken);
        var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken);
        if (rowsAffected.Equals(0))
            return;

        var cachedLeads = await _cacheProvider.GetAsync<IEnumerable<LeadData>>(
            _leadsCachingPolicy.CacheKey,
            cancellationToken);

        if (!cachedLeads?.Any() ?? false)
            return;

        var leads = cachedLeads!.ToList();
        var outdatedLead = leads.SingleOrDefault(ld => ld.Id == lead.Id);
        if (outdatedLead.IsNull)
            return;

        leads.Remove(outdatedLead);
        leads.Add(lead.MapToMessageContract());

        await _cacheProvider.SetAsync<IEnumerable<LeadData>>(
                _leadsCachingPolicy.CacheKey,
                leads,
                _leadsCachingPolicy.TtlInSeconds,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<Lead, bool>> matchCriteria,
        CancellationToken cancellationToken = default)
    {
        var cachedLeads = await _cacheProvider.GetAsync<IEnumerable<LeadData>>(
            _leadsCachingPolicy.CacheKey,
            cancellationToken);

        if (!cachedLeads?.Any() ?? false)
            return cachedLeads!
                    .Select(ld => ld.MapToEntity())
                    .Any(matchCriteria.Compile());

        return await _leadRepository.ExistsAsync(matchCriteria, cancellationToken);
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
        => await _cacheProvider.RemoveAsync(_leadsCachingPolicy.CacheKey, cancellationToken);
}
