using Application.Core.Contracts.Persistence;
using Application.Core.Contracts.Repository.Prospecting;
using Domain.Prospecting.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DataQuerying;
using Shared.FrameworkExtensions;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Infrastructure.Repository.Prospecting;

internal sealed class LeadRepository : RepositoryBase<Lead>, ILeadRepository
{
    private readonly ILeadManagerDbContext _leadDbContext;

    public LeadRepository(ILeadManagerDbContext leadDbContext)
    {
        _leadDbContext = leadDbContext;
    }

    public override async Task<IEnumerable<Lead>> GetAllAsync(CancellationToken cancellationToken = default)
       => await _leadDbContext
                    .Leads
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

    public override async Task<Lead?> GetByIdAsync(
        Guid id,
        bool? bypassCacheLayer = false,
        CancellationToken cancellationToken = default)
        => await _leadDbContext.Leads.FindAsync([id], cancellationToken: cancellationToken);

    public override async Task<PagedList<Lead>> GetAsync(
        PaginationOptions? paginationOptions,
        QueryOptions? queryOptions = default,
        CancellationToken cancellationToken = default)
        => await Task.FromResult(
                    _leadDbContext
                        .Leads
                        .AsNoTracking()
                        .ToFilteredOrderedPagedList(
                            GenerateSearchQueryExpression,
                            queryOptions ?? new(),
                            paginationOptions?.SortColumn ?? nameof(Lead.RazaoSocial),
                            paginationOptions?.SortDirection ?? ListSortDirection.Ascending,
                            paginationOptions));


	public override async Task AddAsync(Lead lead, CancellationToken cancellationToken = default)
        => await _leadDbContext.Leads.AddAsync(lead, cancellationToken);

    public override async Task AddRangeAsync(IEnumerable<Lead> leads, CancellationToken cancellationToken = default)
        => await _leadDbContext.Leads.AddRangeAsync(leads, cancellationToken);

    public override async Task RemoveAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        if (await _leadDbContext.Leads.AnyAsync(ld => ld.Id == lead.Id, cancellationToken))
            _leadDbContext.Leads.Remove(lead);
    }

    public override async Task UpdateAsync(Lead lead, byte[] rowVersion, CancellationToken cancellationToken = default)
    {
        if (await _leadDbContext.Leads.AnyAsync(ld => ld.Id == lead.Id, cancellationToken))
        {
            _leadDbContext.SetStateToModified(lead);
            _leadDbContext.SetConcurrencyToken(lead, nameof(Lead.RowVersion), rowVersion);
        }
    }

    public override async Task<bool> ExistsAsync(
        Expression<Func<Lead, bool>> matchCriteria,
        CancellationToken cancellationToken = default)
        => await _leadDbContext
                    .Leads
                    .AsNoTracking()
                    .AnyAsync(matchCriteria, cancellationToken);

    public async Task<PagedList<LeadsFile>> GetLeadsFilesAsync(
        PaginationOptions paginationOptions,
        CancellationToken cancellationToken = default)
        => await Task.FromResult(PagedList<LeadsFile>.Paginate(
                            _leadDbContext.LeadsFiles.AsNoTracking().OrderByDescending(it => it.CreatedAt),
                            paginationOptions));

    public async Task AddLeadsFileAsync(LeadsFile leadsFile, CancellationToken cancellationToken = default)
        => await _leadDbContext.LeadsFiles.AddAsync(leadsFile, cancellationToken);

    public async Task<LeadsFile?> GetLeadsFileByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _leadDbContext.LeadsFiles.FindAsync([id], cancellationToken: cancellationToken);

    public async Task RemoveLeadsFilesByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        => await _leadDbContext.LeadsFiles.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync(cancellationToken);

	private static IQueryable<Lead> GenerateSearchQueryExpression(
		IQueryable<Lead> queryable,
		QueryOptions? queryOptions)
	{
        if (queryOptions is null)
            return queryable;

		if (!string.IsNullOrWhiteSpace(queryOptions.Term))
			queryable = queryable.Where(it => it.Cnpj.Contains(queryOptions.Term, StringComparison.InvariantCultureIgnoreCase) ||
											  it.RazaoSocial.Contains(queryOptions.Term, StringComparison.InvariantCultureIgnoreCase));

		return queryable;
	}
}
