using AspNetCore.Identity.MongoDbCore.Models;
using IAMServer.Clients.LeadWebApp.Security;
using IAMServer.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace IAMServer.Core.HostingServices;

public sealed class IdentitySeedHostedService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<IdentitySeedHostedService> _logger;

    public IdentitySeedHostedService(ILogger<IdentitySeedHostedService> logger, IServiceScopeFactory serviceScopeFactory)
        => (_logger, _serviceScopeFactory) = (logger, serviceScopeFactory);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<IdentitySeedHostedService>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        _logger.LogInformation("Initializing database users seeding...");

        var adminRoleExists = await roleManager.RoleExistsAsync(LeadManagerAppConstants.Roles.Administrators);
        if (!adminRoleExists)
        {
            var adminsRole = new ApplicationRole { Name = LeadManagerAppConstants.Roles.Administrators };
            await roleManager.CreateAsync(adminsRole);
        }

        if (!userManager.Users.Any())
        {
            const string FulanoEmail = "fulano.silva@leadmanager.com.br";
            var fulano = new ApplicationUser
            {
                Claims = new List<MongoClaim>
                {
                    new MongoClaim() { Type = LeadManagerAppConstants.Claims.LDM, Value = LeadManagerAppConstants.Claims.Read },
                    new MongoClaim() { Type = LeadManagerAppConstants.Claims.LDM, Value = LeadManagerAppConstants.Claims.Insert },
                    new MongoClaim() { Type = LeadManagerAppConstants.Claims.LDM, Value = LeadManagerAppConstants.Claims.BulkInsert },
                    new MongoClaim() { Type = LeadManagerAppConstants.Claims.LDM, Value = LeadManagerAppConstants.Claims.Update },
                    new MongoClaim() { Type = JwtClaimTypes.Email, Value = FulanoEmail }
                },
                UserName = "fulano.silva",
                Email = FulanoEmail,
                PhoneNumber = "932459987",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            const string BeltranoEmail = "beltrano.silva@leadmanager.com.br";
            var beltrano = new ApplicationUser
            {
                Claims = new List<MongoClaim>
                {
                    new MongoClaim() { Type = LeadManagerAppConstants.Claims.LDM, Value = LeadManagerAppConstants.Claims.Read },
                    new MongoClaim() { Type = JwtClaimTypes.Email, Value = BeltranoEmail }
                },
                UserName = "beltrano.silva",
                Email = BeltranoEmail,
                PhoneNumber = "921654987",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            const string AdministradorEmail = "admin.sistema@leadmanager.com.br";
            var administrador = new ApplicationUser
            {
                Claims = new List<MongoClaim>
                {
                    new MongoClaim() { Type = LeadManagerAppConstants.Claims.LDM, Value = LeadManagerAppConstants.Claims.Delete },
                    new MongoClaim() { Type = LeadManagerAppConstants.Claims.LDM, Value = LeadManagerAppConstants.Claims.Read },
                    new MongoClaim() { Type = JwtClaimTypes.Email, Value = AdministradorEmail }
                },
                UserName = "admin.sistema",
                Email = AdministradorEmail,
                PhoneNumber = "954876522",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            await userManager.CreateAsync(fulano, "FuL@n0!1991");
            await userManager.CreateAsync(beltrano, "B3ltr@n01993");
            await userManager.CreateAsync(administrador, "@Dm1n!01+=");
            await userManager.AddToRoleAsync(administrador, LeadManagerAppConstants.Roles.Administrators);
        }

        _logger.LogInformation("Database users seeding finished.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
