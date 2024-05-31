using Application.Core.Contracts.Repository.Caching;
using Application.Core.Contracts.Repository.Security;
using Application.Security.OneTimePassword.Commands.HandleOneTimePassword;
using CrossCutting.Caching;

namespace Infrastructure.Repository.Security;

internal sealed class OneTimePasswordRepository : IOneTimePasswordRepository
{
    private readonly ICacheProvider _cacheProvider;
    private readonly OneTimePasswordCachingPolicy _oneTimePasswordCachingPolicy;

    public OneTimePasswordRepository(
        ICacheProvider cacheProvider,
        OneTimePasswordCachingPolicy oneTimePasswordCachingPolicy)
    {
        _cacheProvider = cacheProvider;
        _oneTimePasswordCachingPolicy = oneTimePasswordCachingPolicy;
    }

    public async Task<OneTimePasswordDto?> GetAsync(
        Guid userId,
        string resource,
        CancellationToken cancellationToken = default)
        => await _cacheProvider.GetAsync<OneTimePasswordDto>($"{userId}_{resource}", cancellationToken);

    public async Task SaveAsync(
        OneTimePasswordDto oneTimePasswordDto,
        CancellationToken cancellationToken = default)
        => await _cacheProvider.SetAsync(
            $"{oneTimePasswordDto.UserId}_{oneTimePasswordDto.Resource}",
            oneTimePasswordDto,
            _oneTimePasswordCachingPolicy.TtlInSeconds,
            cancellationToken);

    public async Task RemoveAsync(
        Guid userId,
        string resource,
        CancellationToken cancellationToken = default)
        => await _cacheProvider.SetAsync<OneTimePasswordDto>(
                                   $"{userId}_{resource}",
                                   new(Guid.NewGuid(), default!, DateTime.MinValue, default!),
                                   ttlInSeconds: 1,
                                   cancellationToken: cancellationToken);
}
