using CrossCutting.Monitoring.Configuration;
using IAMServer.Clients.LeadWebApp.Configuration;
using IAMServer.Clients.LeadWebApp.Security;
using IAMServer.Core.HostingServices;
using IAMServer.Entities;
using IAMServer.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Shared.Settings;
using static IAMServer.Clients.LeadWebApp.Security.LeadManagerAppConstants;

namespace IAMServer.Core.Configuration;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddWebAppServices(this WebApplicationBuilder builder)
    {
        var leadManagerWebAppClientSettings = builder.Configuration
                                                        .GetSection(nameof(LeadWebAppClientSettings))
                                                        .Get<LeadWebAppClientSettings>()!;

        builder.Services.AddControllers();
        builder.Services.AddRazorPages(); // <<< I had to add it manually
        builder.Services.AddHostedService<IdentitySeedHostedService>();
        builder.Services.AddCors(corsOptions =>
        {
            //Lead Manager Web Application Client
            corsOptions.AddPolicy(LeadManagerAppConstants.CorsPolicyName,
                policyBuilder =>
                {
                    if (builder.Environment.IsDevelopment())
                        policyBuilder.AllowAnyOrigin();

                    policyBuilder
                           .WithOrigins(leadManagerWebAppClientSettings.BaseUrl)
                           .AllowAnyHeader()
                           .WithMethods(leadManagerWebAppClientSettings.Methods.Split(','))
                           .AllowCredentials()
                           .Build();
                }
            );
        });

		var iamServerApiSettings = builder.Configuration
                                            .GetSection(nameof(IAMServerApiSettings))
											.Get<IAMServerApiSettings>()!;

		builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme =
                    options.DefaultAuthenticateScheme =
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = iamServerApiSettings.Authority;
                    options.Audience = iamServerApiSettings.Audience;
                    options.RequireHttpsMetadata = iamServerApiSettings.RequireHttpsMetadata;
                });

        builder.Services.AddAuthorization(policyOptions =>
        {
			policyOptions.AddPolicy(Policies.LeadManagerAdministrativeTasksPolicy, policy =>
			{
				policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
				policy.RequireAuthenticatedUser();
				policy.RequireRole(Roles.Administrators);
			});
		});

		return builder;
    }

    public static WebApplicationBuilder AddIS4(this WebApplicationBuilder builder)
    {
        //Configures MongoDb to register identifiers as Guids instead of ObjectIds
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        var dataSourceSettings = builder.Configuration.GetSection(nameof(DataSourceSettings)).Get<DataSourceSettings>()!;
        builder.Services
                    .AddIdentity<ApplicationUser, ApplicationRole>(identityOptions =>
                    {
                        identityOptions.SignIn.RequireConfirmedAccount = false; //https://www.tektutorialshub.com/asp-net-core/asp-net-core-identity-tutorial/#user-managernbsp
                    })
                    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(dataSourceSettings.ConnectionString, dataSourceSettings.DatabaseName);

        var iamServerSettings = builder.Configuration.GetSection(nameof(IAMServerSettings)).Get<IAMServerSettings>()!;
        var leadManagerWebAppClientSettings = builder.Configuration
                                                    .GetSection(nameof(LeadWebAppClientSettings))
                                                    .Get<LeadWebAppClientSettings>()!;
        var identityServerBuilder = builder.Services
                        .AddIdentityServer(srvOpts =>
                        {
                            //Provides with verbose information about the Identity Server logs
                            if (builder.Environment.IsDevelopment())
                            {
                                srvOpts.Events.RaiseSuccessEvents =
                                srvOpts.Events.RaiseFailureEvents =
                                srvOpts.Events.RaiseErrorEvents = true;
                            }
                        })
                        .AddAspNetIdentity<ApplicationUser>()
                        .AddInMemoryApiScopes(iamServerSettings.ApiScopes)
                        .AddInMemoryApiResources(iamServerSettings.ApiResources)
                        .AddInMemoryClients(iamServerSettings.Clients)
                        .AddInMemoryIdentityResources(iamServerSettings.IdentityResources)
                        .AddProfileService<LeadWebAppProfileService>();

        //builder.Services.AddLocalApiAuthentication(); //Use it in case you expose an user management API here in this server

        //In production, usually a digital certificate is used to sign the tokens produced by the Identity Server.
        //For sandboxed environments, a key with a password does the trick by using an auto-generated file
        if (builder.Environment.IsDevelopment())
            identityServerBuilder = identityServerBuilder.AddDeveloperSigningCredential(); // this can be attached to the code above, if you wanted

        builder.Services.AddIAMServerMonitoring(builder.Configuration);

        return builder;
    }
}
