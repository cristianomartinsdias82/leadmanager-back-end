using Shared.DataPagination;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Shared.FrameworkExtensions;

public static class EnumerableExtensions
{
    public static IList<T> ToSortedList<T>(
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
            ListSortDirection.Ascending => items.OrderBy(sortExpression).ToList(),
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
        Func<IEnumerable<T>, IEnumerable<TOutput>> map)
        => PagedList<T>.Paginate(items, paginationOptions, map);

    public static PagedList<T> ToSortedPagedList<T>(
        this IEnumerable<T> items,
        PaginationOptions paginationOptions,
        string sortColumn,
        ListSortDirection sortDirection)
        => items
            .ToSortedList(sortColumn, sortDirection)
            .ToPagedList(paginationOptions);

    public static PagedList<TOutput> ToSortedPagedList<T, TOutput>(
        this IEnumerable<T> items,
        string sortColumn,
        ListSortDirection sortDirection,
        PaginationOptions paginationOptions,
        Func<IEnumerable<T>, IEnumerable<TOutput>> map)
        => items
            .ToSortedList(sortColumn, sortDirection)
            .ToPagedList(paginationOptions, map);
}
