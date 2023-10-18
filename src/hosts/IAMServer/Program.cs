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