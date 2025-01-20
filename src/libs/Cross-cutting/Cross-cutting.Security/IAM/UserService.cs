using CrossCutting.Security.Authentication.JsonWebTokens;
using Microsoft.AspNetCore.Http;

namespace CrossCutting.Security.IAM;

internal sealed class UserService : IUserService
{
    private readonly HttpContext _httpContext;

	public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext!;
    }

	public bool CurrentUserIsAdministrator => _httpContext.IsInAdministratorsRole();

	public Guid? GetUserId()
        => _httpContext.GetUserId();

    public string? GetUserEmail()
        => _httpContext.GetUserEmail();
}