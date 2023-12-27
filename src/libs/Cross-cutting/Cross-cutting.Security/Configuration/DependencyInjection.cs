using CrossCutting.Security.Authentication.JsonWebTokens.Configuration;
using CrossCutting.Security.Authorization;
using CrossCutting.Security.IAM.Configuration;
using CrossCutting.Security.Secrecy.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwtAuthentication(configuration);
        services.AddAuthorizationCheckers(configuration);
        services.AddUserServices(configuration);
        services.AddSecrecyServices(configuration);

        return services;
    }

    private static IServiceCollection AddAuthorizationCheckers(this IServiceCollection services, IConfiguration configuration)
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