using Microsoft.AspNetCore.Mvc;

namespace CrossCutting.Security.Authorization;

public abstract class RequiredPermissionsAttribute : TypeFilterAttribute
{
    public RequiredPermissionsAttribute(
            AuthorizationCheckStrategy checkStrategy,
            params string[] requiredPermissions)
        : base(typeof(PermissionCheckAuthorizationFilter))
    {
        Arguments = new object[] { requiredPermissions, checkStrategy };
    }
}
