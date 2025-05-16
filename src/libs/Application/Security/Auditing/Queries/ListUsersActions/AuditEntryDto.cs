namespace Application.Security.Auditing.Queries.ListUsersActions;

public sealed record AuditEntryDto(
	Guid Id,
	string UserId,
	string Action,
	string OldData,
	string? NewData,
	DateTimeOffset ActionDateTime
	);