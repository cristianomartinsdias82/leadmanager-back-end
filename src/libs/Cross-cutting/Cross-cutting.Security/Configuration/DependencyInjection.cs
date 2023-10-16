using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CrossCutting.Security.Authentication.JsonWebTokens.Configuration;

namespace CrossCutting.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwtAuthentication(configuration);

        return services;
    }
}