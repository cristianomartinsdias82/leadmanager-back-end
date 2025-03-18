using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.Security.Authentication.JsonWebTokens.Configuration;

internal static class JwtAuthenticationServicesExtensions
{
    public static AuthenticationBuilder AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        => services.ConfigureOptions<JwtAuthenticationConfigureOptions>()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        //This event is raised everytime an authenticated request is sent
                        //OnTokenValidated = context =>
                        //{
                        //    var sp = context.HttpContext.RequestServices;
                        //    var logger = sp.GetService<ILogger<AuthenticationBuilder>>();

                        //    logger?.LogInformation("Token validated.");

                        //    return Task.CompletedTask;
                        //}
                    };
                });
}