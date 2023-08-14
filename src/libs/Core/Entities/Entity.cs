namespace Core.Entities;

public abstract class Entity : IAuditableEntity<Guid, Guid?>
{
    public virtual Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedUserId { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedUserId { get; set; }
}