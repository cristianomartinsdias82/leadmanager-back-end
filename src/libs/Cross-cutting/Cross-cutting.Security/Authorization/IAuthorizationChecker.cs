using Microsoft.AspNetCore.Http;

namespace CrossCutting.Security.Authorization;

internal interface IAuthorizationChecker
{
    bool IsAuthorized(HttpContext httpContext, string[] permissions);
}
