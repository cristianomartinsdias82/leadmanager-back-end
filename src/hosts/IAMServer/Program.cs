//using IAMServer.Entities;
//using IAMServer.Settings;
//using MongoDB.Bson;
//using MongoDB.Bson.Serialization;
//using MongoDB.Bson.Serialization.Serializers;
//using IAMServer.Clients.LeadWebApp.Security;
//using IAMServer.Clients.LeadWebApp.Configuration;
//using IAMServer.Core.HostingServices;
using IAMServer.Clients.LeadWebApp.Configuration;
using IAMServer.Clients.LeadWebApp.Security;
using IAMServer.Core.Configuration;

namespace IAMServer;

public class Program
{
    public static void Main(string[] args)
    {
        //Build and add services
        var builder = WebApplication
                        .CreateBuilder(args)
                        .AddWebAppServices()
                        .AddIS4();

        //builder.Services.AddControllers();
        //builder.Services.AddRazorPages(); // <<< I had to add it manually
        //builder.Services.AddHostedService<IdentitySeedHostedService>();

        //BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        //var dataSourceSettings = builder.Configuration.GetSection(nameof(DataSourceSettings)).Get<DataSourceSettings>()!;
        //builder.Services
        //            .AddIdentity<ApplicationUser, ApplicationRole>(identityOptions =>
        //            {
        //                //https://www.tektutorialshub.com/asp-net-core/asp-net-core-identity-tutorial/#user-managernbsp
        //                identityOptions.SignIn.RequireConfirmedAccount = false;
        //            })
        //            .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(dataSourceSettings.ConnectionString, dataSourceSettings.DatabaseName);

        //var iamServerSettings = builder.Configuration.GetSection(nameof(IAMSettings)).Get<IAMSettings>()!;
        //var leadManagerWebAppClientSettings = builder.Configuration
        //                                            .GetSection(nameof(LeadWebAppClientSettings))
        //                                            .Get<LeadWebAppClientSettings>()!;
        //var identityServerBuilder = builder.Services
        //                .AddIdentityServer(srvOpts =>
        //                {
        //                    //Provides with verbose information about the Identity Server logs
        //                    if (builder.Environment.IsDevelopment())
        //                    {
        //                        srvOpts.Events.RaiseSuccessEvents =
        //                        srvOpts.Events.RaiseFailureEvents =
        //                        srvOpts.Events.RaiseErrorEvents = true;
        //                    }
        //                })
        //                .AddAspNetIdentity<ApplicationUser>()
        //                .AddInMemoryApiScopes(iamServerSettings.ApiScopes)
        //                .AddInMemoryApiResources(iamServerSettings.ApiResources)
        //                .AddInMemoryClients(iamServerSettings.Clients)
        //                .AddInMemoryIdentityResources(iamServerSettings.IdentityResources)
        //                .AddProfileService<LeadManagerProfileService>();

        ////builder.Services.AddLocalApiAuthentication(); //Use it in case you expose an user management API here in this server

        ////In production, usually a digital certificate is used to sign the tokens produced by the Identity Server.
        ////For sandboxed environments, a key with a password does the trick by using an auto-generated file
        //if (builder.Environment.IsDevelopment())
        //    identityServerBuilder = identityServerBuilder.AddDeveloperSigningCredential(); // this can be attached to the code above, if you wanted

        //builder.Services.AddCors(corsOptions =>
        //{
        //    //Lead Manager Web Application Client
        //    corsOptions.AddPolicy(leadManagerWebAppClientSettings.CorsPolicyName,
        //        policyBuilder =>
        //        {
        //            if (builder.Environment.IsDevelopment())
        //                policyBuilder.AllowAnyOrigin();

        //            policyBuilder
        //                   .WithOrigins(leadManagerWebAppClientSettings.BaseUrl)
        //                   .AllowAnyHeader()
        //                   .WithMethods(leadManagerWebAppClientSettings.Methods.Split(','))
        //                   .AllowCredentials()
        //                   .Build();
        //        }
        //    );
        //});


        var app = builder.Build();


        // Configure the Authorization Server HTTP request pipeline.
        app.UseStaticFiles(); // <<< I had to add it manually

        app.UseHttpsRedirection();

        app.UseCors(LeadManagerAppConstants.CorsPolicyName); // <<< I had to add it manually

        app.UseIdentityServer(); // <<< I had to add it manually

        //app.UseAuthentication(); //Use it in case you expose an user management API here in this server

        app.UseAuthorization();

        app.MapRazorPages(); // <<< I had to add it manually

        app.MapControllers();

        app.Run();
    }
}