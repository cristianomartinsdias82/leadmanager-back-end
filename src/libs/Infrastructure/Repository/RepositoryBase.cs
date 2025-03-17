using Application.Core.Contracts.Repository;
//using Domain.Prospecting.Entities;
//using Domain.Core;
using Shared.DataPagination;
using System.Linq.Expressions;

namespace Infrastructure.Repository;

internal abstract class RepositoryBase<T> : IRepository<T> where T : class//, IEntity
{
    public abstract Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default);

    public abstract Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default);

    public abstract Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);


	public abstract Task<PagedList<T>> GetAsync(
        string? search,
        PaginationOptions paginationOptions,
        CancellationToken cancellationToken = default);

    public abstract Task RemoveAsync(
        T entity,
        byte[] rowVersion,
        CancellationToken cancellationToken = default);

    public abstract Task UpdateAsync(
        T entity,
        byte[] rowVersion,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> ExistsAsync(
        Expression<Func<T, bool>> matchCriteria,
        CancellationToken cancellationToken = default);
}
