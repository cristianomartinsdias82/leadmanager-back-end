namespace Core.Entities;

public interface IEntity
{
    Guid Id { get; set; }

    byte[] RowVersion { get; set; }
}