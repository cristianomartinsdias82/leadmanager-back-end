using Application.Core;
using Application.Core.Contracts.Persistence;
using Application.Core.Contracts.Repository.Security.Auditing;
using Shared.DataPagination;
using System.Linq.Expressions;

namespace Infrastructure.Repository.Security.Auditing;

internal sealed class AuditingRepository : RepositoryBase<AuditEntry>, IAuditingRepository
{
    private readonly ILeadManagerDbContext _leadManagerDbContext;

    public AuditingRepository(ILeadManagerDbContext leadManagerDbContext)
    {
        _leadManagerDbContext = leadManagerDbContext;
    }

    public override async Task AddAsync(AuditEntry entity, CancellationToken cancellationToken = default)
        => await _leadManagerDbContext.AuditEntries.AddAsync(entity, cancellationToken);

    public override Task AddRangeAsync(IEnumerable<AuditEntry> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<bool> ExistsAsync(Expression<Func<AuditEntry, bool>> matchCriteria, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<PagedList<AuditEntry>> GetAsync(PaginationOptions paginationOptions, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<AuditEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveAsync(AuditEntry entity, byte[] rowVersion, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task UpdateAsync(AuditEntry entity, byte[] rowVersion, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
