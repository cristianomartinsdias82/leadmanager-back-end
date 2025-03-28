﻿using CrossCutting.Serialization.ProtoBuf.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.Serialization.Configuration;

public static class DependencyInjection
{
	public static IServiceCollection AddSerialization(this IServiceCollection services)
	{
		services.AddProtoBufSerializationServices();

		return services;
	}
}