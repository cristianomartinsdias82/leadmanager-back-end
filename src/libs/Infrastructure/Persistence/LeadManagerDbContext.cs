using Application.Contracts.Persistence;
using Core.Entities;
using Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
        //https://codewithmukesh.com/blog/audit-trail-implementation-in-aspnet-core/
        foreach (var entry in ChangeTracker
                                .Entries()
                                .Where(e => e.Entity is IAuditableEntity<Guid>))
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    {
                        var auditableEntity = (IAuditableEntity<Guid>)entry.Entity;
                        auditableEntity.CreatedAt = DateTimeOffset.Now;
                        auditableEntity.CreateAuthorId = Guid.NewGuid(); //identityService.GetUserId();

                        break;
                    }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}