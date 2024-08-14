using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Domain.Core;
using System.Text.Json;
using Application.Core;
using CrossCutting.Security.IAM;

namespace Infrastructure.Persistence;

internal sealed class AppendAuditEntryInterceptor : SaveChangesInterceptor
{
    private readonly IUserService _userService;

    public AppendAuditEntryInterceptor(IUserService userService)
    {
        _userService = userService;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AppendAuditEntry(
            eventData.Context,
            _userService,
            cancellationToken);

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private static void AppendAuditEntry(
        DbContext? context,
        IUserService userService,
        CancellationToken cancellationToken = default)
    {
        if (context is null || context is not LeadManagerDbContext leadManagerContext)
            return;

        var entities = context!.ChangeTracker.Entries<IAuditableEntity>()?.ToList() ?? new List<EntityEntry<IAuditableEntity>>();
        DateTimeOffset entryDateTime = default;
        string serializedEntityOldValue;
        string serializedEntityNewValue;
        Guid entityId;
        SystemActions action = SystemActions.LeadRegistration;
        var entryCount = entities?.Count() ?? 0;
        var shouldAddEntry = false;

        foreach (EntityEntry<IAuditableEntity> entry in entities!)
        {
            shouldAddEntry = false;
            serializedEntityNewValue = null!;
            serializedEntityOldValue = null!;
            entityId = (Guid)entry.Property(nameof(IAuditableEntity.Id)).CurrentValue!;

            if (entry.State is EntityState.Added)
            {
                shouldAddEntry = true;
                entryDateTime = (DateTimeOffset)entry.Property(nameof(IAuditableEntity.CreatedAt)).CurrentValue!;
                serializedEntityOldValue = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                action = entryCount > 1 ? SystemActions.BulkLeadRegistration : SystemActions.LeadRegistration;
            }

            else if (entry.State is EntityState.Modified)
            {
                shouldAddEntry = true;
                entryDateTime = (DateTimeOffset)entry.Property(nameof(IAuditableEntity.UpdatedAt)).CurrentValue!;
                serializedEntityOldValue = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                serializedEntityNewValue = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                action = SystemActions.LeadDataUpdate;
            }

            else if (entry.State is EntityState.Deleted)
            {
                shouldAddEntry = true;
                entryDateTime = DateTime.UtcNow;
                serializedEntityOldValue = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                action = SystemActions.LeadExclusion;
            }

            if (shouldAddEntry)
                leadManagerContext.AuditEntries.AddAsync(
                    AuditEntry.Create(
                        entryDateTime,
                        userService.GetUserEmail() ?? userService.GetUserId()?.ToString() ?? "Não identificado",
                        action,
                        entityId,
                        entry.Metadata.ClrType.FullName,
                        serializedEntityOldValue,
                        serializedEntityNewValue),
                    cancellationToken);
        }
    }
}
