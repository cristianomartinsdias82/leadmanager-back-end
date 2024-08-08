using Application.Security.OneTimePassword.Commands.HandleOneTimePassword;

namespace Application.Core.Contracts.Repository.Security.OneTimePassword;

public interface IOneTimePasswordRepository
{
    Task SaveAsync(
        OneTimePasswordDto oneTimePasswordDto,
        CancellationToken cancellationToken = default);
    
    Task<OneTimePasswordDto?> GetAsync(
        Guid userId,
        string resource,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(
        Guid userId,
        string resource,
        CancellationToken cancellationToken = default);
}