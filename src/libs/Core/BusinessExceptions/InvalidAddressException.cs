namespace Core.BusinessExceptions;

public sealed class InvalidAddressException : BusinessException
{
    public InvalidAddressException(string? message) : base(message) { }
}
