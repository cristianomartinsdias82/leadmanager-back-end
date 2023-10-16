using AspNetCore.Identity.MongoDbCore.Models;
using IAMServer.Clients.LeadWebApp.Security;
using IAMServer.Entities;
using Microsoft.AspNetCore.Identity;

namespace IAMServer.Extensions.Seeders;

public static class IdentitySeeder
{
    public async static Task<IHost> SeedUsers(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var adminRoleExists = await roleManager.RoleExistsAsync(LeadManagerRoles.Administrators);
        if (!adminRoleExists)
        {
            var adminsRole = new ApplicationRole { Name = LeadManagerRoles.Administrators };
            await roleManager.CreateAsync(adminsRole);
        }

        if (!userManager.Users.Any())
        {
            var user1 = new ApplicationUser
            {
                Claims = new List<MongoClaim>
                {
                    new MongoClaim() { Type = LeadManagerClaims.LDM, Value = LeadManagerClaims.Read },
                    new MongoClaim() { Type = LeadManagerClaims.LDM, Value = LeadManagerClaims.Insert },
                    new MongoClaim() { Type = LeadManagerClaims.LDM, Value = LeadManagerClaims.BulkInsert },
                    new MongoClaim() { Type = LeadManagerClaims.LDM, Value = LeadManagerClaims.Update }
                },
                UserName = "cristiano.dias",
                NormalizedUserName = "CRISTIANO.DIAS",
                Email = "crisdi@gmail.com",
                PhoneNumber = "2345678901",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var user2 = new ApplicationUser
            {
                Claims = new List<MongoClaim>
                {
                    new MongoClaim() { Type = LeadManagerClaims.LDM, Value = LeadManagerClaims.Read }
                },
                UserName = "adriana.sena",
                NormalizedUserName = "ADRIANA.SENA",
                Email = "adridesena@gmail.com",
                PhoneNumber = "0123456789",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var adminUser = new ApplicationUser
            {
                Claims = new List<MongoClaim>
                {
                    new MongoClaim() { Type = LeadManagerClaims.LDM, Value = LeadManagerClaims.Delete },
                    new MongoClaim() { Type = LeadManagerClaims.LDM, Value = LeadManagerClaims.Read }
                },
                UserName = "marlene.sena",
                NormalizedUserName = "MARLENE.SENA",
                Email = "marlenesena@gmail.com",
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            await userManager.CreateAsync(user1, "M1nhoc0!@_+=");
            await userManager.CreateAsync(user2, "M1nhoc@!0_+=");
            await userManager.CreateAsync(adminUser, "1ItsR@ining!Man_+=");
            await userManager.AddToRoleAsync(adminUser, LeadManagerRoles.Administrators);
        }
        
        return host;
    }
}
