using Application.Core.Contracts.Repository.Caching;
using Application.Core.Contracts.Repository.Prospecting;
using Application.Core.Contracts.Repository.UnitOfWork;
using Application.Prospecting.Leads.Shared;
using CrossCutting.Caching;
using CrossCutting.MessageContracts;
using Domain.Prospecting.Entities;
using Infrastructure.Repository.Prospecting;
using LanguageExt;
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
	private readonly Func<CancellationToken, Task<IEnumerable<LeadData>>> _getData;

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

		_getData = async ct =>
		{
			var leads = await _leadRepository.GetAllAsync(ct);

			return leads.Select(ld => ld.MapToMessageContract());
		};
	}

	public async Task<IEnumerable<Lead>> GetAllAsync(CancellationToken cancellationToken = default)
		=> await _leadRepository.GetAllAsync(cancellationToken);

	public async Task<PagedList<Lead>> GetAsync(
		PaginationOptions paginationOptions,
		string? search = null,
		CancellationToken cancellationToken = default)
	{
		var cachedLeads = await _cacheProvider.GetOrCreateAsync(
			_leadsCachingPolicy.CacheKey,
			async ct => await _getData(ct),
			tags: [_leadsCachingPolicy.CacheKey],
			cancellationToken);

		return LeadRepository.GenerateSearchQueryExpression(
					cachedLeads
						.Select(ld => ld.MapToEntity())
						.AsQueryable(),
					search)
					.ToSortedPagedList(
						paginationOptions.SortColumn ?? nameof(Lead.RazaoSocial),
						paginationOptions.SortDirection,
						paginationOptions,
						x => x);
	}

	public async Task<Lead?> GetByIdAsync(
		Guid id,
		bool? bypassCacheLayer = false,
		CancellationToken cancellationToken = default)
	{
		if (bypassCacheLayer.HasValue && bypassCacheLayer.Value)
			return await _leadRepository.GetByIdAsync(id, bypassCacheLayer, cancellationToken);

		var cachedLeads = await GetAsync(PaginationOptions.SinglePage(),
										 cancellationToken: cancellationToken);

		return cachedLeads.Items.SingleOrDefault(ld => ld.Id == id);
	}

	public async Task AddAsync(Lead lead, CancellationToken cancellationToken = default)
	{
		await _leadRepository.AddAsync(lead, cancellationToken);
		_unitOfWork.AddNonObtrusiveCommitSuccessfulPostAction(async (ct) =>
		{
			var leads = await GetAsync(PaginationOptions.SinglePage(),
									   cancellationToken: ct);

			await UpdateCacheAsync(
				_cacheProvider,
				_leadsCachingPolicy,
				[.. leads.Items, lead],
				cancellationToken);
		});
	}

	public async Task AddRangeAsync(IEnumerable<Lead> leadsBatch, CancellationToken cancellationToken = default)
	{
		await _leadRepository.AddRangeAsync(leadsBatch, cancellationToken);
		_unitOfWork.AddNonObtrusiveCommitSuccessfulPostAction(async (ct) =>
		{
			var leads = await GetAsync(PaginationOptions.SinglePage(),
									   cancellationToken: ct);

			await UpdateCacheAsync(
				_cacheProvider,
				_leadsCachingPolicy,
				[.. leads.Items, .. leadsBatch],
				cancellationToken);
		});
		}

	public async Task RemoveAsync(Lead lead, CancellationToken cancellationToken = default)
	{
		await _leadRepository.RemoveAsync(lead, cancellationToken);
		_unitOfWork.AddNonObtrusiveCommitSuccessfulPostAction(async (ct) =>
		{
			var leads = await GetAsync(PaginationOptions.SinglePage(),
									   cancellationToken: ct);

			await UpdateCacheAsync(
					_cacheProvider,
					_leadsCachingPolicy,
					[.. leads.Items.Where(ld => ld.Id != lead.Id)],
					cancellationToken);
		});
	}

	public async Task UpdateAsync(Lead lead, byte[] rowVersion, CancellationToken cancellationToken = default)
	{
		await _leadRepository.UpdateAsync(lead, rowVersion, cancellationToken);
		_unitOfWork.AddNonObtrusiveCommitSuccessfulPostAction(async (ct) =>
		{
			var leads = await GetAsync(PaginationOptions.SinglePage(),
									   cancellationToken: ct);

			await UpdateCacheAsync(
			_cacheProvider,
			_leadsCachingPolicy,
			[.. leads.Items.Where(ld => ld.Id != lead.Id), lead],
			cancellationToken);
		});
	}

	public async Task<bool> ExistsAsync(
		Expression<Func<Lead, bool>> matchCriteria,
		CancellationToken cancellationToken = default)
	{
		var cachedLeads = await GetAsync(
									PaginationOptions.SinglePage(),
									cancellationToken: cancellationToken);

		if (cachedLeads.ItemCount > 0)
			return cachedLeads.Items.Any(matchCriteria.Compile());

		return await _leadRepository.ExistsAsync(matchCriteria, cancellationToken);
	}

	public async Task ClearAsync(CancellationToken cancellationToken = default)
		=> await _cacheProvider.RemoveAsync(_leadsCachingPolicy.CacheKey, cancellationToken);
	//=> await _cacheProvider.RemoveByTagAsync(_leadsCachingPolicy.CacheKey, cancellationToken);

	public async Task<PagedList<LeadsFile>> GetLeadsFilesAsync(PaginationOptions paginationOptions, CancellationToken cancellationToken = default)
		=> await _leadRepository.GetLeadsFilesAsync(paginationOptions, cancellationToken);

	public async Task AddLeadsFileAsync(LeadsFile leadsFile, CancellationToken cancellationToken = default)
		=> await _leadRepository.AddLeadsFileAsync(leadsFile, cancellationToken);

	public async Task<LeadsFile?> GetLeadsFileByIdAsync(Guid id, CancellationToken cancellationToken = default)
		=> await _leadRepository.GetLeadsFileByIdAsync(id, cancellationToken);

	public async Task RemoveLeadsFilesByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
		=> await _leadRepository.RemoveLeadsFilesByIdsAsync(ids, cancellationToken);

	private static async Task UpdateCacheAsync(
		ICacheProvider cacheProvider,
		LeadsCachingPolicy cachingPolicy,
		List<Lead> leads,
		CancellationToken cancellationToken = default)
		=> await cacheProvider.SetAsync(
				cachingPolicy.CacheKey,
				leads.Select(ld => ld.MapToMessageContract()),
				cachingPolicy.TtlInSeconds,
				tags: [cachingPolicy.CacheKey],
				cancellationToken);
}
