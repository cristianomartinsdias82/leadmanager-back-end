namespace Application.Core.Contracts.Repository.Caching;

public sealed record CachingPoliciesSettings
{
    public LeadsCachingPolicy LeadsCachingPolicy { get; init; } = default!;
    public AddressesCachingPolicy AddressesCachingPolicy { get; init; } = default!;
    public OneTimePasswordCachingPolicy OneTimePasswordCachingPolicy { get; init; } = default!;
}

public record CachingPolicy(string CacheKey, int TtlInSeconds);

public sealed record LeadsCachingPolicy : CachingPolicy
{
    public LeadsCachingPolicy(string CacheKey, int TtlInSeconds) : base(CacheKey, TtlInSeconds) { }
}

public sealed record AddressesCachingPolicy : CachingPolicy
{
    public AddressesCachingPolicy(string CacheKey, int TtlInSeconds) : base(CacheKey, TtlInSeconds) { }
}

public sealed record OneTimePasswordCachingPolicy : CachingPolicy
{
    public int ExpirationTimeInSeconds { get; init; }
    public int ExpirationTimeOffsetInSeconds { get; init; }

    public OneTimePasswordCachingPolicy(int ExpirationTimeInSeconds, int ExpirationTimeOffsetInSeconds, int TtlInSeconds) : base(default!, TtlInSeconds)
    {
        this.ExpirationTimeInSeconds = ExpirationTimeInSeconds;
        this.ExpirationTimeOffsetInSeconds = ExpirationTimeOffsetInSeconds;
    }
}