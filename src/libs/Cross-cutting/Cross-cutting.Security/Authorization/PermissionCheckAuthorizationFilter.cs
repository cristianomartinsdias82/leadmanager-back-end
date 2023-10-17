using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.Security.Authorization;

public class PermissionCheckAuthorizationFilter : IAuthorizationFilter
{
    private readonly string[] _requiredPermissions;
    private readonly AuthorizationCheckStrategy _checkStrategy;

    public PermissionCheckAuthorizationFilter(string[] requiredPermissions, AuthorizationCheckStrategy checkStrategy)
    {
        _requiredPermissions = requiredPermissions;
        _checkStrategy = checkStrategy;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext is null)
            throw new InvalidOperationException();

        var authorizationChecker = context.HttpContext
                                          .RequestServices
                                          .GetRequiredService<AuthorizationCheckResolver>()(_checkStrategy);
        
        if (!authorizationChecker.IsAuthorized(context.HttpContext, _requiredPermissions))
            context.Result = new UnauthorizedResult();
    }
}
