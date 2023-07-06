namespace Core.Entities;

public abstract class Entity : IEntity<Guid>, IAuditableEntity<Guid>
{
    public virtual Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public virtual Guid CreateAuthorId { get; set; }
}