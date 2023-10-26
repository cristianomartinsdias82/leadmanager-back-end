using Application.Core.Contracts.Persistence;
using Application.Core.Contracts.Repository.UnitOfWork;

namespace Infrastructure.Repository.UnitOfWork;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly ILeadManagerDbContext _leadManagerDbContext;

    public UnitOfWork(ILeadManagerDbContext leadManagerDbContext)
    {
        _leadManagerDbContext = leadManagerDbContext;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => await _leadManagerDbContext.SaveChangesAsync(cancellationToken);
}
