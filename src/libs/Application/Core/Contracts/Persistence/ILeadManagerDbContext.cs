using Domain.Core;
using Domain.Prospecting.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Core.Contracts.Persistence;

public interface ILeadManagerDbContext : IDisposable, IAsyncDisposable
{
    DbSet<Lead> Leads { get; set; }
    DbSet<AuditEntry> AuditEntries { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    void SetConcurrencyToken<T>(T entity, string propertyName, byte[] rowVersion) where T : IEntity;
    void SetStateToModified<T>(T entity) where T : IEntity;
}