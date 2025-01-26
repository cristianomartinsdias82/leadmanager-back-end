using IdentityModel;
using Microsoft.AspNetCore.Http;

namespace CrossCutting.Security.Authentication.JsonWebTokens;

internal static class HttpContextExtensions
{
	private const string AdministratorsRole = "Administrators"; //Refer to hosts/authorization-server/IAMServer/Clients/LeadWebApp/LeadWebAppConstants.cs

	public static Guid? GetUserId(this HttpContext httpContext)
        => Guid.TryParse(httpContext.User?.FindFirst(JwtClaimTypes.Subject)?.Value, out var userId) ? userId : default;

    public static string? GetUserEmail(this HttpContext httpContext)
        => httpContext.User?.FindFirst(JwtClaimTypes.Email)?.Value;

    public static bool IsInAdministratorsRole(this HttpContext httpContext)
        => httpContext.User?.IsInRole(AdministratorsRole) ?? false;
}
