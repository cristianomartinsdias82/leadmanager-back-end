﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LeadManagerApi.Core.Configuration.Security;

internal static class LeadManagerApiSecurityConfiguration
{
    public static class Policies
    {
        public const string LeadManagerDefaultPolicy = "LeadManagerDefaultPolicy";
        public const string LeadManagerRemovePolicy = "LeadManagerRemovePolicy";
        public const string LeadManagerCorsPolicy = "LeadWebAppCorsPolicy";
    }

    private static class Roles
    {
        public const string Administrators = "Administrators";
    }

    public static class ClaimTypes
    {
        public const string LDM = "ldm";
    }

    public static class Claims
    {
        public const string Read = "leadmanager.read";
        public const string Insert = "leadmanager.insert";
        public const string BulkInsert = "leadmanager.bulk_insert";
        public const string Update = "leadmanager.update";
        public const string Delete = "leadmanager.delete";
    }

    public static class Permissions
    {
        public const string Read = Claims.Read;
        public const string Insert = Claims.Insert;
        public const string BulkInsert = Claims.BulkInsert;
        public const string Update = Claims.Update;
        public const string Delete = Claims.Delete;
    }

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
