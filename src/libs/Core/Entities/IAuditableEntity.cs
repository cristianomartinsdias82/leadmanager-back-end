namespace Core.Entities;

public interface IAuditableEntity : IEntity
{
    DateTimeOffset CreatedAt { get; set; }
    Guid CreatedUserId { get; set; }

    DateTimeOffset? UpdatedAt { get; set; }
    Guid? UpdatedUserId { get; set; }
}