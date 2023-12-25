namespace Application.Security.OneTimePassword.Commands.HandleOneTimePassword;

public enum OneTimePasswordHandlingOperationResult
{
    CodeGeneratedSuccessfully = 1,
    InvalidCode = 2,
    ExpiredCode = 3,
    ValidCode = 4,
    Error = 5
}