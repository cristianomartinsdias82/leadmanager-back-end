using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts.Persistence;

public interface ILeadManagerDbContext : IDisposable, IAsyncDisposable
{
    DbSet<Lead> Leads { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}