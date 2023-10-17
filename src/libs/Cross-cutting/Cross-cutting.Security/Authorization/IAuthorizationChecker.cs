using Microsoft.AspNetCore.Http;

namespace CrossCutting.Security.Authorization;

public interface IAuthorizationChecker
{
    bool IsAuthorized(HttpContext httpContext, string[] permissions);
}
