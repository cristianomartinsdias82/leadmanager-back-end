namespace Application.Security.OneTimePassword.Commands.GenerateOneTimePassword;

public sealed record GenerateOneTimePasswordDto(
    Guid UserId,
    string Resource,
    DateTime CreatedOn,
    string Code
);
