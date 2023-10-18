namespace CrossCutting.Security.Authorization;

public sealed class RequiredAllPermissionsAttribute : RequiredPermissionsAttribute
{
    public RequiredAllPermissionsAttribute(params string[] requiredPermissions)
        : base(AuthorizationCheckStrategy.All, requiredPermissions) { }
}
