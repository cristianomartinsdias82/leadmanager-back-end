using Application.Core;
using CrossCutting.Security.IAM;
using Domain.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace Infrastructure.Persistence;

internal sealed class AppendAuditEntryInterceptor : SaveChangesInterceptor
{
    private readonly IUserService _userService;
    private readonly TimeProvider _timeProvider;

    public AppendAuditEntryInterceptor(
        IUserService userService,
        TimeProvider timeProvider)
    {
        _userService = userService;
        _timeProvider = timeProvider;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AppendAuditEntry(
            eventData.Context,
            _userService,
            _timeProvider,
            cancellationToken);

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

	private static void AppendAuditEntry(
		DbContext? context,
		IUserService userService,
		TimeProvider timeProvider,
		CancellationToken cancellationToken = default)
	{
		if (context is null || context is not LeadManagerDbContext leadManagerContext)
			return;

		var entities = context!.ChangeTracker.Entries<IAuditableEntity>()?.ToList() ?? [];

		//Bulk registration counts as one single entry in the auditing table
		if (entities.Count > 1 && entities!.TrueForAll(it => it.State == EntityState.Added))
		{
			leadManagerContext
				.AuditEntries
				.AddAsync(
					AuditEntry.Create(
						(DateTimeOffset)entities!.First().Property(nameof(IAuditableEntity.CreatedAt)).CurrentValue!,
						userService.GetUserEmail() ?? userService.GetUserId()?.ToString() ?? "Não identificado",
						SystemActions.BulkLeadRegistration,
						default,
						entities!.First().Metadata.ClrType.FullName,
						JsonSerializer.Serialize<object>(entities.Select(it => it.CurrentValues.ToObject())),
						default),
					cancellationToken);

			return;
		}

		DateTimeOffset entryDateTime = default;
		string serializedEntityOldValue;
		string serializedEntityNewValue;
		Guid entityId;
		SystemActions action = SystemActions.LeadRegistration;
		
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
				action = SystemActions.LeadRegistration;
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
				entryDateTime = timeProvider.GetLocalNow();
				serializedEntityOldValue = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
				action = SystemActions.LeadExclusion;
			}

			if (shouldAddEntry)
				leadManagerContext
					.AuditEntries
					.AddAsync(
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
