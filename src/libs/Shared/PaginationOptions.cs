namespace Shared;

public sealed record PaginationOptions
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int ItemCount { get; set; }
    public int PageCount
        => (int)Math.Ceiling((double)ItemCount / PageSize);
}