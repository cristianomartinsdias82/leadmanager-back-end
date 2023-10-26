namespace Application.Core.Contracts.Repository.Caching;

public sealed record CachingPoliciesSettings
{
    public LeadsPolicy LeadsPolicy { get; init; } = default!;
    public AddressesPolicy AddressesPolicy { get; init; } = default!;
}

public record CachingPolicy(string CacheKey, int TtlInSeconds);

public  record LeadsPolicy : CachingPolicy
{
    public LeadsPolicy(string CacheKey, int TtlInSeconds) : base(CacheKey, TtlInSeconds) { }
};

public sealed record AddressesPolicy : CachingPolicy
{
    public AddressesPolicy(string CacheKey, int TtlInSeconds) : base(CacheKey, TtlInSeconds) { }
};