using HealthChecks.UI.Client;
using IAMServer.Clients.LeadWebApp.Configuration;
using IAMServer.Clients.LeadWebApp.Security;
using IAMServer.Core.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

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

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseStaticFiles(); // <<< I had to add it manually

        app.UseHttpsRedirection();

        app.MapHealthChecks("/_health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseCors(LeadManagerAppConstants.CorsPolicyName); // <<< I had to add it manually

        app.UseIdentityServer(); // <<< I had to add it manually

        app.UseAuthentication(); //Use it in case you expose an user management API here in this server

		app.UseAuthorization();

        app.MapRazorPages(); // <<< I had to add it manually

        app.MapControllers();
        app.MapEndpoints();

        app.Run();
    }
}