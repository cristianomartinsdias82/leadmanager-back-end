using IAMServer.Entities;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IAMServer.Clients.LeadWebApp.Security;

/// <summary>
/// This class is what makes claims data appear on the generated Jwt
/// (Note that this is registered in Program.cs via .AddProfileService<LeadManagerProfileService>() command line
/// </summary>
//https://stackoverflow.com/questions/44761058/how-to-add-custom-claims-to-access-token-in-identityserver4
public class LeadManagerProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public LeadManagerProfileService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        if (user is null)
            return;

        if (user.Roles.Any())
        {
            var userRole = await _roleManager.FindByIdAsync(user.Roles.First().ToString());
            context.IssuedClaims.Add(new Claim("role", userRole!.Name!));
        }

        context.IssuedClaims.AddRange(user.Claims.Select(cl => new Claim(cl.Type, cl.Value)).ToList());
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);

        context.IsActive = user?.EmailConfirmed ?? false;
    }
}
