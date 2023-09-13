namespace Core.Entities;

public interface IEntity
{
    Guid Id { get; set; }

    //DateTimeOffset RevisionNumber { get; set; }
    byte[] RowVersion { get; set; }
}