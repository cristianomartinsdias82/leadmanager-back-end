using Microsoft.AspNetCore.Http;

namespace CrossCutting.Security.Authorization;

public abstract class AuthorizationChecker : IAuthorizationChecker
{
    public virtual bool IsAuthorized(HttpContext httpContext, string[] permissions)
    {
        if (httpContext is null)
            throw new InvalidOperationException("HttpContext cannot be null");

        return httpContext.User.Identity?.IsAuthenticated ?? false;
    }
}
