using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Core.Configuration.Security;

internal static class LeadManagerApiSecurityConfiguration
{
    public static void SetPermissionPolicies(AuthorizationOptions policyOptions)
    {
        policyOptions.AddPolicy(Policies.LeadManagerDefaultPolicy, policy =>
        {
            policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(
                ClaimTypes.LDM,
                Permissions.Read,
                Permissions.BulkInsert,
                Permissions.Insert,
                Permissions.Update,
                Permissions.Delete);
        });

        policyOptions.AddPolicy(Policies.LeadManagerRemovePolicy, policy =>
        {
            policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
            policy.RequireAuthenticatedUser();
            policy.RequireRole(Roles.Administrators);
            policy.RequireClaim(ClaimTypes.LDM, Permissions.Delete);
        });
    }

    public static void SetAuthorizationForSwagger(SwaggerGenOptions swaggerGenOptions)
    {
        swaggerGenOptions.AddSecurityDefinition(
            JwtBearerDefaults.AuthenticationScheme,
            new OpenApiSecurityScheme()
            {
                Description = $"Jwt Authorization header using the Bearer scheme. Example: \"Authorization: {JwtBearerDefaults.AuthenticationScheme} {{token}}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

        swaggerGenOptions.AddSecurityRequirement(
            new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Id = JwtBearerDefaults.AuthenticationScheme,
                            Type = ReferenceType.SecurityScheme
                        }
                    }, new List<string>()
                }
            });
    }

    public static void SetCorsPolicy(CorsOptions corsOptions, LeadManagerApiSettings apiSettings)
        => corsOptions.AddPolicy(Policies.LeadManagerCorsPolicy,
                builder =>
                {
                    builder.WithOrigins(apiSettings.Cors_AllowedOrigins.Split(','))
                           .WithMethods("POST", "GET", "PUT", "DELETE", "HEAD", "OPTIONS")
                           .AllowAnyHeader()
                           .AllowCredentials()
                           .Build();
                });
}
