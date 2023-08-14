namespace Core.Entities;

public interface IAuditableEntity<UserCreationTKey, UserUpdateTKey>
{
    DateTimeOffset CreatedAt { get; set; }
    UserCreationTKey CreatedUserId { get; set; }

    DateTimeOffset? UpdatedAt { get; set; }
    UserUpdateTKey? UpdatedUserId { get; set; }
}