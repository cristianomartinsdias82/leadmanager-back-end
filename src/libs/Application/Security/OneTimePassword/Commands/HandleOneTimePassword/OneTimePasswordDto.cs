namespace Application.Security.OneTimePassword.Commands.HandleOneTimePassword;

public sealed record OneTimePasswordDto(
    Guid UserId,
    string Resource,
    DateTime CreatedOn,
    string Code
);
