using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CrossCutting.Serialization.ProtoBuf.Configuration;

internal static class ProtoBufSerializationServicesExtensions
{
	public static IServiceCollection AddProtoBufSerializationServices(this IServiceCollection services)
	{
		services.TryAddSingleton<IDataSerialization, ProtoBufSerializer>();

		return services;
	}
}