using CrossCutting.Caching.Hybrid.Configuration;
using Microsoft.Extensions.Caching.Hybrid;
using Polly;
using Polly.Retry;

namespace CrossCutting.Caching.Hybrid;

//https://www.nuget.org/packages/Microsoft.Extensions.Caching.Hybrid/9.3.0#readme-body-tab
//https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid?view=aspnetcore-9.0
//https://www.youtube.com/watch?v=wlNHmZHQ5sY ("HybridCache is finally stable!!! FAST L1 + L2 Cache in .NET")
internal sealed class HybridCacheProvider : CacheProvider
{
	private const string GetDataDelegateArgumentCannotBeNull = "Get data delegate argument cannot be null.";
	private readonly HybridCache _cache;
	private readonly HybridCacheProviderSettings _settings;

    public HybridCacheProvider(
		HybridCache cache,
		HybridCacheProviderSettings settings)
    {
        _cache = cache;
		_settings = settings;
    }

	public override Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default)
	{
        ArgumentNullException.ThrowIfNull(GetDataDelegateArgumentCannotBeNull);

		return ExecuteAsync(
			    async () =>
				    await _cache.GetOrCreateAsync(
                        key,
                        (ct) => factory(ct),
                        tags: tags,
						cancellationToken: cancellationToken));
	}

	public override Task<T> GetAsync<T>(
		string key,
		CancellationToken cancellationToken = default)
	    => ExecuteAsync(
				async () =>
					await _cache.GetOrCreateAsync(
						key,
						(_) => ValueTask.FromResult<T>(default!),
						cancellationToken: cancellationToken));

	public override Task SetAsync<T>(
        string key,
        T item,
        int ttlInSeconds = 300,
        IEnumerable<string>? tags = default,
        CancellationToken cancellationToken = default)
		=> ExecuteAsync(
                async () =>
                    await _cache.SetAsync(
                                    key,
                                    item,
                                    new HybridCacheEntryOptions
                                    {
                                        Expiration = TimeSpan.FromSeconds(ttlInSeconds),
                                        LocalCacheExpiration = TimeSpan.FromSeconds(ttlInSeconds)
                                    },
                                    tags,
                                    cancellationToken: cancellationToken));

    public override Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
        => ExecuteAsync(
				async () => await _cache.RemoveAsync(key, cancellationToken));

	public override Task RemoveByTagAsync(
        string tag,
        CancellationToken cancellationToken = default)
	    => ExecuteAsync(
                async () => await _cache.RemoveByTagAsync(tag, cancellationToken));

    private Task ExecuteAsync(Func<Task> func)
        => GetPolicy(_settings.ConnectionAttemptsMaxCount)
			.ExecuteAsync(async () => await func());

	private Task<T> ExecuteAsync<T>(
		Func<Task<T>> func)
		=> GetPolicy(_settings.ConnectionAttemptsMaxCount)
			.ExecuteAsync(async () => await func());

    private static AsyncRetryPolicy GetPolicy(int maxAttempts)
        => Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                maxAttempts,
				count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)));
}
