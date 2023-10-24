using Domain.Prospecting.Entities;
using CrossCutting.Security.IAM;
using Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;
using Domain.Core;
using Application.Core.Contracts.Persistence;

namespace Infrastructure.Persistence;

public sealed class LeadManagerDbContext : DbContext, ILeadManagerDbContext
{
    private readonly IUserService _userService;

    public LeadManagerDbContext(
        DbContextOptions<LeadManagerDbContext> options,
        IUserService userService) : base(options)
    {
        _userService = userService;
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
        var userId = _userService.GetUserId();
        if (userId is null)
            throw new InvalidProgramException();

        var now = DateTimeOffset.UtcNow;

        //https://codewithmukesh.com/blog/audit-trail-implementation-in-aspnet-core/
        foreach (var entry in ChangeTracker
                                .Entries()
                                .Where(e => e.Entity is IAuditableEntity))
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    {
                        var auditableEntity = (IAuditableEntity)entry.Entity;
                        auditableEntity.CreatedAt = now;
                        auditableEntity.CreatedUserId = userId.Value;

                        break;
                    }
                case EntityState.Modified:
                    {
                        var auditableEntity = (IAuditableEntity)entry.Entity;
                        auditableEntity.UpdatedAt = now;
                        auditableEntity.UpdatedUserId = userId;

                        break;
                    }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public void SetConcurrencyToken<T>(T entity, string propertyName, byte[] rowVersion) where T : IEntity
    {
        Entry(entity).Property(propertyName).OriginalValue = rowVersion;
    }
}