namespace Application.Core.Contracts.Repository.Caching;

public interface ICachingLeadRepository
{
    Task ClearAsync(CancellationToken cancellationToken = default);
}
