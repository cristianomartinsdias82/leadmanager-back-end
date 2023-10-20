using Microsoft.AspNetCore.Http;
using CrossCutting.Security.Authentication.JsonWebTokens;

namespace CrossCutting.Security.IAM;

internal sealed class UserService : IUserService
{
    private readonly HttpContext _httpContext;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext is null)
            throw new InvalidProgramException();

        _httpContext = httpContextAccessor.HttpContext;
    }

    public Guid? GetUserId()
        => _httpContext.GetUserId();

    public string? GetUserEmail()
        => _httpContext.GetUserEmail();
}
