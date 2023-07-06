namespace Core.Entities;

public interface IAuditableEntity<UserTKey>
{
    DateTimeOffset CreatedAt { get; set; }
    //DateTimeOffset? UpdatedAt { get; set; }
    //DateTimeOffset? RemovedAt { get; set; }
    UserTKey CreateAuthorId { get; set; }
    //UserTKey? UpdateAuthorId { get; set; }
    //UserTKey? RemoveAuthorId { get; set; }
}