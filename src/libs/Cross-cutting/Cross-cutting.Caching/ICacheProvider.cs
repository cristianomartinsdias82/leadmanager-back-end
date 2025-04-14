namespace CrossCutting.Caching;

public interface ICacheProvider
{
    Task<T> GetAsync<T>(
		string key,
		CancellationToken cancellationToken = default);

	Task<T> GetOrCreateAsync<T>(
		string key,
		Func<CancellationToken, ValueTask<T>> factory,
		IEnumerable<string>? tags = null,
		CancellationToken cancellationToken = default);

	Task SetAsync<T>(
		string key,
		T item,
		int ttlInSeconds = 300,
		IEnumerable<string>? tags = default,
		CancellationToken cancellationToken = default);

    Task RemoveAsync(
		string key,
		CancellationToken cancellationToken = default);

    Task RemoveByTagAsync(
		string tag,
		CancellationToken cancellationToken = default);
}