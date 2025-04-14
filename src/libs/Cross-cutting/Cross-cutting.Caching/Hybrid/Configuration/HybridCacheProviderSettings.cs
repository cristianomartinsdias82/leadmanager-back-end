namespace CrossCutting.Caching.Hybrid.Configuration;

public sealed record HybridCacheProviderSettings
(
	int MaximumPayloadBytes,
	int ConnectionAttemptsMaxCount
);