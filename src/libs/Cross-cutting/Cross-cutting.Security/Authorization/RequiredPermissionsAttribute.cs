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

public sealed class RequiredAllPermissionsAttribute : RequiredPermissionsAttribute
{
    public RequiredAllPermissionsAttribute(params string[] requiredPermissions)
        : base(AuthorizationCheckStrategy.All, requiredPermissions) { }
}

public sealed class RequiredAnyPermissionAttribute : RequiredPermissionsAttribute
{
    public RequiredAnyPermissionAttribute(params string[] requiredPermissions)
        : base(AuthorizationCheckStrategy.Any, requiredPermissions) { }
}