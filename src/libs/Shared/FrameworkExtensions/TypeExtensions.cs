namespace Shared.FrameworkExtensions;

public static class TypeExtensions
{
	/// <summary>
	/// Indica se um tipo implementa ou é decorado com um determinado tipo
	/// Muito útil para saber se uma classe implementa/é decorada com' uma interface, por exemplo.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public static bool ImplementsOrIsDecoratedWith<T>(this Type type)
	{
		return typeof(T).IsAssignableFrom(type);
	}
}
