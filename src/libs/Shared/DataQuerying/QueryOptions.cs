namespace Shared.DataQuerying;

public sealed record QueryOptions
{
	public string? UserId { get; init; }
	public string? Term { get; init; }
	public DateTimeOffset? StartDate { get; init; }
	public DateTimeOffset? EndDate { get; init; }
}