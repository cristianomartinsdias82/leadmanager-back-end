using Shared.DataQuerying;
using System.Linq.Expressions;

namespace Application.Core.Contracts.Repository;

public interface IRepository<T> where T : class//, IEntity
{
	Task AddAsync(
		T entity,
		CancellationToken cancellationToken = default);

	Task<T?> GetByIdAsync(
		Guid id,
		bool? bypassCacheLayer = false,
		CancellationToken cancellationToken = default);

	Task<PagedList<T>> GetAsync(
		PaginationOptions? paginationOptions = default,
		QueryOptions? queryOptions = default,
		CancellationToken cancellationToken = default);

	Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

	Task AddRangeAsync(
		IEnumerable<T> entities,
		CancellationToken cancellationToken = default);

	Task RemoveAsync(
		T entity,
		CancellationToken cancellationToken = default);

	Task UpdateAsync(
		T entity,
		byte[] rowVersion,
		CancellationToken cancellationToken = default);

	Task<bool> ExistsAsync(
		Expression<Func<T, bool>> matchCriteria,
		CancellationToken cancellationToken = default);
}