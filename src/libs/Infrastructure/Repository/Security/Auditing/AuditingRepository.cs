using Application.Core;
using Application.Core.Contracts.Persistence;
using Application.Core.Contracts.Repository.Security.Auditing;
using Microsoft.EntityFrameworkCore;
using Shared.DataQuerying;
using Shared.FrameworkExtensions;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Infrastructure.Repository.Security.Auditing;

internal sealed class AuditingRepository : RepositoryBase<AuditEntry>, IAuditingRepository
{
    private readonly ILeadManagerDbContext _leadManagerDbContext;

    public AuditingRepository(ILeadManagerDbContext leadManagerDbContext)
    {
        _leadManagerDbContext = leadManagerDbContext;
    }

    public override async Task AddAsync(AuditEntry entity, CancellationToken cancellationToken = default)
        => await _leadManagerDbContext
                    .AuditEntries
                    .AddAsync(entity, cancellationToken);

    public override Task AddRangeAsync(IEnumerable<AuditEntry> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<bool> ExistsAsync(Expression<Func<AuditEntry, bool>> matchCriteria, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

	public override Task<IEnumerable<AuditEntry>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public override Task<AuditEntry?> GetByIdAsync(
        Guid id,
		bool? bypassCacheLayer = false,
		CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveAsync(AuditEntry entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task UpdateAsync(AuditEntry entity, byte[] rowVersion, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override async Task<PagedList<AuditEntry>> GetAsync(
        PaginationOptions? paginationOptions = default,
        QueryOptions? queryOptions = default,
        CancellationToken cancellationToken = default)
        => await Task.FromResult(_leadManagerDbContext
            .AuditEntries
            .AsNoTracking()
            .ToFilteredOrderedPagedList(
                GenerateSearchQueryExpression,
                queryOptions ?? new(),
				paginationOptions?.SortColumn ?? nameof(AuditEntry.ActionDateTime),
				paginationOptions?.SortDirection ?? ListSortDirection.Descending,
				paginationOptions));

	private static IQueryable<AuditEntry> GenerateSearchQueryExpression(
        IQueryable<AuditEntry> queryable,
        QueryOptions? queryOptions)
    {
        if (queryOptions is null)
            return queryable;

		if (!string.IsNullOrWhiteSpace(queryOptions.Term))
        {
            var systemsActions = EnumExtensions.GetOptionsSimilarTo<SystemActions>(queryOptions.Term);

            queryable = queryable.Where(it => systemsActions.Count > 0 ? systemsActions.Contains(it.Action) : systemsActions.Contains(SystemActions.Unknown));
		}

		if (!string.IsNullOrWhiteSpace(queryOptions.UserId))
			queryable = queryable.Where(it => it.UserId == queryOptions.UserId);

		if (queryOptions.StartDate.HasValue)
			queryable = queryable.Where(it => it.ActionDateTime.Date >= queryOptions.StartDate.Value.Date);

		if (queryOptions.EndDate.HasValue)
			queryable = queryable.Where(it => it.ActionDateTime.Date <= queryOptions.EndDate.Value.Date);

		return queryable;
    }
}