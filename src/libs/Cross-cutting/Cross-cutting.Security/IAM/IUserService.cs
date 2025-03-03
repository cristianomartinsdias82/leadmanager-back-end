﻿namespace CrossCutting.Security.IAM;

public interface IUserService
{
    Guid? GetUserId();
    string? GetUserEmail();
    bool CurrentUserIsAdministrator { get; }
}
