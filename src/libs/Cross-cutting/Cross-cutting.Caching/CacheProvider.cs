namespace CrossCutting.Caching;

internal abstract class CacheProvider : ICacheProvider
{
    public abstract Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    public abstract Task SetAsync<T>(string key, T item, int ttlInSeconds = 300, CancellationToken cancellationToken = default);
    public abstract Task RemoveAsync<T>(string key, CancellationToken cancellationToken = default);
}
