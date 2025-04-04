﻿//using Domain.Core;
using Shared.DataPagination;
using System.Linq.Expressions;

namespace Application.Core.Contracts.Repository;

public interface IRepository<T> where T : class//, IEntity
{
    Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedList<T>> GetAsync(
		string? search,
		PaginationOptions paginationOptions,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(
        T entity,
        byte[] rowVersion,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        T entity,
        byte[] rowVersion,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Expression<Func<T,bool>> matchCriteria,
        CancellationToken cancellationToken = default);
}