namespace Core.BusinessExceptions;

public sealed class InvalidZipCodeException : BusinessException
{
    public InvalidZipCodeException(string? message) : base(message) { }
}
