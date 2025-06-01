using CrossCutting.Security.Authentication.JsonWebTokens.Configuration;
using CrossCutting.Security.Authorization;
using CrossCutting.Security.IAM.Configuration;
using CrossCutting.Security.Secrecy.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.Security.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddJwtAuthentication();
        services.AddAuthorizationCheckers();
        services.AddUserServices();
        services.AddSecrecyServices();

        return services;
    }

    private static IServiceCollection AddAuthorizationCheckers(this IServiceCollection services)
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