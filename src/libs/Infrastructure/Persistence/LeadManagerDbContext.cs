using Application.Core;
using Application.Core.Contracts.Persistence;
using CrossCutting.Security.IAM;
using Domain.Core;
using Domain.Prospecting.Entities;
using Domain.Reporting;
using Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class LeadManagerDbContext : DbContext, ILeadManagerDbContext
{
    private readonly IUserService _userService;
	private readonly TimeProvider _timeProvider;

	public LeadManagerDbContext(
        DbContextOptions<LeadManagerDbContext> options,
        IUserService userService,
        TimeProvider timeProvider) : base(options)
    {
		_timeProvider = timeProvider;
        _userService = userService;
	}

    public DbSet<Lead> Leads { get; set; }
    public DbSet<AuditEntry> AuditEntries { get; set; }
	public DbSet<LeadsFile> LeadsFiles { get; set; }
    public DbSet<ReportGenerationRequest> ReportGenerationRequests { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(LeadEntityMetadata.DatabaseSchemaName);

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
        => SaveChangesAsync()
            .GetAwaiter()
            .GetResult();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userService.GetUserId() ?? throw new InvalidProgramException();

		var now = _timeProvider.GetLocalNow();

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
                        auditableEntity.CreatedUserId = userId;

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

    public void SetStateToModified<T>(T entity) where T : IEntity
    {
        Entry(entity).State = EntityState.Modified;
    }
}