using CrossCutting.Caching.Redis.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using ProtoBuf;

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

    public override async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        T? item = default;
        return await Policy.Handle<Exception>()
                .WaitAndRetryAsync(_settings.ConnectionAttemptsMaxCount, count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)))
                .ExecuteAsync(async () =>
                {
                    var itemData = await _cache.GetAsync(key, cancellationToken);

                    if (itemData is not null)
                        item = FromByteArray(itemData, item!);

                    return item!;
                });
    }

    public override async Task SetAsync<T>(string key, T item, int ttlInSeconds = 300, CancellationToken cancellationToken = default)
    {
        await Policy.Handle<Exception>()
                .WaitAndRetryAsync(_settings.ConnectionAttemptsMaxCount, count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)))
                .ExecuteAsync(async () =>
                {
                    await _cache.SetAsync(
                                    key,
                                    ToByteArray(item),
                                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlInSeconds) },
                                    cancellationToken);
                });
    }

    public override async Task RemoveAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        await Policy.Handle<Exception>()
                .WaitAndRetryAsync(_settings.ConnectionAttemptsMaxCount, count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)))
                .ExecuteAsync(async () =>
                {
                    await _cache.RemoveAsync(
                                    key,
                                    cancellationToken);
                });
    }

    private static byte[] ToByteArray<T>(T item)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, item);
        return ms.ToArray();
    }

    private static T FromByteArray<T>(ReadOnlyMemory<byte> span, T defaultValue = default!)
        => Serializer.Deserialize(span, defaultValue);
}
