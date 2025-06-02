using System.Reflection;

namespace Shared.Helpers;

public static class EmbeddedResourceHelper
{
	public static string GetContent(Type type, string resourceFullPath, bool throwIfNotFound = false)
		=> GetContent(type.Assembly, resourceFullPath, throwIfNotFound);

	public static string GetContent(Assembly assembly, string resourceFullPath, bool throwIfNotFound = false)
	{
		var stream = assembly.GetManifestResourceStream(resourceFullPath);
		if (stream is null)
		{
			if (throwIfNotFound)
				throw new FileNotFoundException($"Erro ao carregar o recurso '{resourceFullPath}' solicitado: não encontrado.");

			return string.Empty;
		}

		var streamContent = string.Empty;
		using var reader = new StreamReader(stream);
		streamContent = reader.ReadToEnd();
		stream.Close();

		return streamContent;
	}
}
