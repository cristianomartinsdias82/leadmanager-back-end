using Shared.DataQuerying;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Shared.FrameworkExtensions;

public static class EnumerableExtensions
{
    public static IList<T> ToOrderedList<T>(
        this IEnumerable<T> items,
        string sortColumn,
        ListSortDirection sortDirection)
    {
        ArgumentException.ThrowIfNullOrEmpty(sortColumn);

        var parameter = Expression.Parameter(typeof(T), "param");
        var property = Expression.Property(parameter, sortColumn);
        var sortExpression = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter)
                                       .Compile();

        return sortDirection switch
        {
            ListSortDirection.Ascending => [.. items.OrderBy(sortExpression)],
            _ => items.OrderByDescending(sortExpression).ToList()
        };
    }

    public static PagedList<T> ToPagedList<T>(
        this IEnumerable<T> items,
        PaginationOptions paginationOptions)
        => PagedList<T>.Paginate(items, paginationOptions);

    public static PagedList<TOutput> ToPagedList<T, TOutput>(
        this IEnumerable<T> items,
        PaginationOptions paginationOptions,
        Func<T, TOutput> map)
        => PagedList<T>.Paginate(items, paginationOptions, map);

    public static PagedList<T> ToOrderedPagedList<T>(
        this IEnumerable<T> items,
        PaginationOptions paginationOptions,
        string sortColumn,
        ListSortDirection sortDirection)
        => items
            .ToOrderedList(sortColumn, sortDirection)
            .ToPagedList(paginationOptions);

    public static PagedList<TOutput> ToOrderedPagedList<T, TOutput>(
        this IEnumerable<T> items,
        string sortColumn,
        ListSortDirection sortDirection,
        PaginationOptions paginationOptions,
        Func<T, TOutput> map)
        => items
            .ToOrderedList(sortColumn, sortDirection)
            .ToPagedList(paginationOptions, map);

    public static PagedList<T> ToFilteredOrderedPagedList<T>(
      this IQueryable<T> items,
	  Func<IQueryable<T>, QueryOptions, IQueryable<T>> filterFactory,
      QueryOptions queryOptions,
	  string sortColumn,
      ListSortDirection sortDirection,
      PaginationOptions? paginationOptions)
	{
        var filter = filterFactory(items, queryOptions);

        return filter.ToOrderedPagedList(
                        paginationOptions ?? PaginationOptions.SinglePage(),
                        sortColumn,
                        sortDirection);
	}

	public static PagedList<TOutput> ToFilteredOrderedPagedList<T, TOutput>(
	  this IQueryable<T> items,
	  Func<IQueryable<T>, QueryOptions?, IQueryable<T>> filterFactory,
	  QueryOptions? queryOptions,
	  string sortColumn,
	  ListSortDirection sortDirection,
	  PaginationOptions? paginationOptions,
	  Func<T, TOutput> map)
	{
		var filter = filterFactory(items, queryOptions);

		return filter.ToOrderedPagedList(
                        sortColumn,
                        sortDirection,
                        paginationOptions ??  PaginationOptions.SinglePage(),
                        map);
	}
}
