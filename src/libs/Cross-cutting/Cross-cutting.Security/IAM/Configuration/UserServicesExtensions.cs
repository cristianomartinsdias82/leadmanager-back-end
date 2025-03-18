using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.Security.IAM.Configuration;

internal static class UserServicesExtensions
{
    public static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUserService, UserService>();
        //services.AddScoped<IUserService, FakeUserService>();

        return services;
    }
}
