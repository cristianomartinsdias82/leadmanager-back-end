using System.ComponentModel;

namespace Shared.DataQuerying;

public sealed record PaginationOptions
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortColumn { get; set; } = default!;
    public ListSortDirection SortDirection { get; set; } = ListSortDirection.Ascending;

    public static PaginationOptions SinglePage()
        => new()
        {
            Page = 1,
            PageSize = 1_000_000,
        };

    public PaginationOptions WithSortColumn(string sortColunm)
    {
        SortColumn = sortColunm;

        return this;
    }

    public PaginationOptions WithSortDirection(ListSortDirection sortDirection)
    {
        SortDirection = sortDirection;

        return this;
    }
}
