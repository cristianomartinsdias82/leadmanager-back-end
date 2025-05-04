using System.Collections;

namespace Shared.DataPagination;

public struct PagedList<T>
{
    public IEnumerable<T> Items { get; init; }
    public int PageCount { get; init; }
    public int ItemCount { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }

    public PagedList<TOther> MapPage<TOther>(Func<T, TOther> map)
        => new()
        {
            ItemCount = ItemCount,
            PageCount = PageCount,
            Items = Items?.Select(map)?.ToList() ?? Enumerable.Empty<TOther>(),
            HasPreviousPage = HasPreviousPage,
            HasNextPage = HasNextPage
        };

    public static PagedList<TOutput> Paginate<TOutput>(IEnumerable<T> items, PaginationOptions paginationOptions, Func<T, TOutput> map)
        => PagedList<TOutput>.Paginate(items.Select(map), paginationOptions);

    public static PagedList<T> Paginate(IEnumerable<T> items, PaginationOptions paginationOptions)
    {
        var start = (paginationOptions.Page - 1) * paginationOptions.PageSize;
        var end = start + paginationOptions.PageSize;
        var range = start..end;
        var pageCount = (int)Math.Ceiling((double)items.Count() / paginationOptions.PageSize);

        return new()
        {
            ItemCount = items.Count(),
            PageCount = pageCount,
            Items = items is IList ? items.Take(range) : items.Take(range).ToList(),
            //Items = items.Take(range).ToList(),
            HasPreviousPage = paginationOptions.Page > 1,
            HasNextPage = paginationOptions.Page < pageCount
        };
    }

    public static PagedList<T> Empty()
        => new() { Items = Enumerable.Empty<T>() };
}