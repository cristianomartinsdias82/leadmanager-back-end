namespace Core.BusinessExceptions;

public class InvalidAddressException : ApplicationException
{
    public InvalidAddressException(string? message) : base(message) { }
}
