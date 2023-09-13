namespace Core.Entities;

public abstract class Entity : IAuditableEntity
{
    public virtual Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedUserId { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedUserId { get; set; }

    //https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/concurrency?view=aspnetcore-7.0
    //https://www.learnentityframeworkcore5.com/handling-concurrency-in-ef-core
    public byte[] RowVersion { get; set; } = default!;
}