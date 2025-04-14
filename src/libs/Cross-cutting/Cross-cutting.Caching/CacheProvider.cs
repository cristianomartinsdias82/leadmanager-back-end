namespace CrossCutting.Caching;

internal abstract class CacheProvider : ICacheProvider
{
    public virtual Task<T> GetAsync<T>(
		string key,
		CancellationToken cancellationToken = default)
		=> throw new NotImplementedException();

	public virtual Task<T> GetOrCreateAsync<T>(
		string key,
		Func<CancellationToken, ValueTask<T>> factory,
		IEnumerable<string>? tags = null,
		CancellationToken cancellationToken = default)
		=> throw new NotImplementedException();

	public virtual Task SetAsync<T>(
		string key,
		T item,
		int ttlInSeconds = 300,
		IEnumerable<string>? tags = default,
		CancellationToken cancellationToken = default)
		 => throw new NotImplementedException();

	public virtual Task RemoveAsync(
		string key,
		CancellationToken cancellationToken = default)
		 => throw new NotImplementedException();

	public virtual Task RemoveByTagAsync(
		string tag,
		CancellationToken cancellationToken = default)
		=> throw new NotImplementedException();
}
