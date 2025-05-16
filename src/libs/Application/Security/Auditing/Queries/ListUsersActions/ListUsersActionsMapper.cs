using Application.Core;

namespace Application.Security.Auditing.Queries.ListUsersActions;

internal static class ListUsersActionsMapper
{
	public static AuditEntryDto Map(this AuditEntry auditEntry)
		=> new(auditEntry.Id,
			auditEntry.UserId,
			auditEntry.Action.ToString(),
			auditEntry.OldData,
			auditEntry.NewData,
			auditEntry.ActionDateTime);
}