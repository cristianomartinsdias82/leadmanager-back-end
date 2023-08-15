namespace CrossCutting.Caching.Redis.Configuration;

public sealed record RedisCacheProviderSettings
(
    string Server,
    string PortNumber,
    int ConnectionAttemptsMaxCount
);