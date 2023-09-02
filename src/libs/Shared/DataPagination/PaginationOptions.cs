using System.ComponentModel;

namespace Shared.DataPagination;

public sealed record PaginationOptions
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortColumn { get; set; } = default!;
    public ListSortDirection SortDirection { get; set; } = ListSortDirection.Ascending;
}
