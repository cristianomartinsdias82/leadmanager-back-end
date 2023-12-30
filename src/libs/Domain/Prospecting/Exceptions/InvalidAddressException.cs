namespace Domain.Prospecting.Exceptions;

public sealed class InvalidAddressException : BusinessException
{
    public InvalidAddressException(string? message) : base(message) { }
}
