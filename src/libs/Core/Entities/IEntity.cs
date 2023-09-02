namespace Core.Entities;

public interface IEntity<TKey> // where TKey : IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
{
    TKey Id { get; set; }
}
