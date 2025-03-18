using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.Security.Secrecy.Configuration;

internal static class SecrecyServicesExtensions
{
    public static IServiceCollection AddSecrecyServices(this IServiceCollection services)
    {
        services.AddScoped<ISecretGenerationService, SecretGeneratorService>();

        return services;
    }
}
