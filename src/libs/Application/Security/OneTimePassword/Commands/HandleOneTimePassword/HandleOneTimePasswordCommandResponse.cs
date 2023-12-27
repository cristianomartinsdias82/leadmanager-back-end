namespace Application.Security.OneTimePassword.Commands.HandleOneTimePassword;

public sealed record HandleOneTimePasswordCommandResponse
{
    public required OneTimePasswordHandlingOperationResult Result { get; init; }
}
