using CrossCutting.Caching.Redis.Configuration;
using CrossCutting.Serialization.ProtoBuf;
using Microsoft.Extensions.Caching.Distributed;
using Polly;

namespace CrossCutting.Caching.Redis;

internal sealed class RedisCacheProvider : CacheProvider
{
    private readonly IDistributedCache _cache;
    private readonly RedisCacheProviderSettings _settings;

    public RedisCacheProvider(
        IDistributedCache cache,
        RedisCacheProviderSettings settings)
    {
        _cache = cache;
        _settings = settings;
    }

    public override Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        T? item = default;
        return Policy.Handle<Exception>()
                .WaitAndRetryAsync(_settings.ConnectionAttemptsMaxCount, count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)))
                .ExecuteAsync(async () =>
                {
                    var itemData = await _cache.GetAsync(key, cancellationToken);
                    if (itemData is not null)
                        item = ProtoBufSerializer.Deserialize<T>(itemData);

                    return item!;
                });
    }

    public override Task SetAsync<T>(string key, T item, int ttlInSeconds = 300, CancellationToken cancellationToken = default)
    {
        return Policy.Handle<Exception>()
                .WaitAndRetryAsync(_settings.ConnectionAttemptsMaxCount, count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)))
                .ExecuteAsync(async () =>
                {
                    await _cache.SetAsync(
                                    key,
                                    item is not null ? ProtoBufSerializer.Serialize(item) : default!,
                                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlInSeconds) },
                                    cancellationToken);
                });
    }

    public override Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return Policy.Handle<Exception>()
                .WaitAndRetryAsync(_settings.ConnectionAttemptsMaxCount, count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)))
                .ExecuteAsync(async () =>
                {
                    await _cache.RemoveAsync(
                                    key,
                                    cancellationToken);
                });
    }
}
