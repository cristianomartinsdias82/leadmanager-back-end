namespace Domain.Prospecting.Exceptions;

public sealed class InvalidZipCodeException : BusinessException
{
    public InvalidZipCodeException(string? message) : base(message) { }
}
