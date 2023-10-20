namespace CrossCutting.Security.Authorization;

public sealed class RequiredAnyPermissionAttribute : RequiredPermissionsAttribute
{
    public RequiredAnyPermissionAttribute(params string[] requiredPermissions)
        : base(AuthorizationCheckStrategy.Any, requiredPermissions) { }
}