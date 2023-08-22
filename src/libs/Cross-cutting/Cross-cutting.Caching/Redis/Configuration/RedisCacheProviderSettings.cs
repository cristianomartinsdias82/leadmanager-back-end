namespace CrossCutting.Caching.Redis.Configuration;

public sealed record RedisCacheProviderSettings
(
    string Server,
    int PortNumber,
    int ConnectionAttemptsMaxCount
);