using Application.Contracts.Persistence;
using Core.Entities;
using Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class LeadManagerDbContext : DbContext, ILeadManagerDbContext
{
    public LeadManagerDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Lead> Leads { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LeadManagerDbContext).Assembly);

        modelBuilder.Entity<Lead>()
                    .HasIndex(ld => ld.Cnpj, LeadEntityMetadata.CnpjColumnIndexName)
                    .IsUnique();

        modelBuilder.Entity<Lead>()
                    .HasIndex(ld => ld.RazaoSocial, LeadEntityMetadata.RazaoSocialColumnIndexName)
                    .IsUnique();

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
        => SaveChangesAsync().GetAwaiter().GetResult();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = Guid.NewGuid();//identityService.GetUserId();
        var now = DateTimeOffset.UtcNow;

        //https://codewithmukesh.com/blog/audit-trail-implementation-in-aspnet-core/
        foreach (var entry in ChangeTracker
                                .Entries()
                                .Where(e => e.Entity is IAuditableEntity<Guid, Guid?>))
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    {
                        var auditableEntity = (IAuditableEntity<Guid, Guid?>)entry.Entity;
                        auditableEntity.CreatedAt = now;
                        auditableEntity.CreatedUserId = currentUserId;

                        break;
                    }
                case EntityState.Modified:
                    {
                        var auditableEntity = (IAuditableEntity<Guid, Guid?>)entry.Entity;
                        auditableEntity.UpdatedAt = now;
                        auditableEntity.UpdatedUserId = currentUserId;

                        break;
                    }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}