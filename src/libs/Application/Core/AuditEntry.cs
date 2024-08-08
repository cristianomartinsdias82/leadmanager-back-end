using Shared.Generators;

namespace Application.Core;

public sealed class AuditEntry
{
    public Guid Id { get; }
    public DateTimeOffset ActionDateTime { get; }
    public string UserId { get; } = default!;
    public SystemActions Action { get; } = default!;
    public Guid? SubjectId { get; }
    public string? FullyQualifiedTypeName { get; } = default!;
    public string OldData { get; private set; } = default!;
    public string? NewData { get; private set; }

    private AuditEntry() { } //This must exist in order to make EF Core work

    private AuditEntry(
        DateTimeOffset actionDateTime,
        string userId,
        SystemActions action,
        Guid? subjectId,
        string? fullyQualifiedTypeName,
        string oldValue,
        string? newValue)
    {
        Id = IdGenerator.NextId();
        ActionDateTime = actionDateTime;
        UserId = userId;
        Action = action;
        SubjectId = subjectId;
        FullyQualifiedTypeName = fullyQualifiedTypeName;
        OldData = oldValue;
        NewData = newValue;
    }

    public static AuditEntry Create(
        DateTimeOffset actionDateTime,
        string userId,
        SystemActions action,
        Guid? subjectId,
        string? fullyQualifiedTypeName,
        string oldValue,
        string? newValue)
        => new(actionDateTime, userId, action, subjectId, fullyQualifiedTypeName, oldValue, newValue);
}
