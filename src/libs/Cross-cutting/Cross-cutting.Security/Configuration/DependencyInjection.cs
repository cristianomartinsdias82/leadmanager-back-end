using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CrossCutting.Security.Authentication.JsonWebTokens.Configuration;
using CrossCutting.Security.Authorization;

namespace CrossCutting.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwtAuthentication(configuration);
        services.AddAuthorizationCheckers(configuration);

        return services;
    }

    public static IServiceCollection AddAuthorizationCheckers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<MustHaveAtLeastOnePermissionAuthorizationChecker>();
        services.AddTransient<MustHaveAllPermissionsAuthorizationChecker>();
        services.AddTransient<AuthorizationCheckResolver>(provider => (checkStrategy)
            => checkStrategy switch
            {
                AuthorizationCheckStrategy.Any => new MustHaveAtLeastOnePermissionAuthorizationChecker(),
                _ => new MustHaveAllPermissionsAuthorizationChecker()
            });

        return services;
    }
}