﻿using Microsoft.AspNetCore.Http;

namespace CrossCutting.Security.Authorization;

internal sealed class MustHaveAllPermissionsAuthorizationChecker : AuthorizationChecker
{
    public override bool IsAuthorized(HttpContext httpContext, string[] permissions)
    {
        if (!base.IsAuthorized(httpContext, permissions))
            return false;

        foreach (var permission in permissions)
        {
            if (!httpContext.User.HasClaim(cl => cl.Value == permission))
                return false;
        }

        return true;
    }
}
