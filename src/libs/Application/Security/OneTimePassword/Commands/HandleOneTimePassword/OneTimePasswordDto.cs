namespace Application.Security.OneTimePassword.Commands.HandleOneTimePassword;

public sealed record OneTimePasswordDto(
    Guid UserId,
    string Resource,
    DateTimeOffset CreatedOn,
    string Code
);
