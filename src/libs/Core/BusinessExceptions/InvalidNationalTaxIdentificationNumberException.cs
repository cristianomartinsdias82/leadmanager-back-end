namespace Core.BusinessExceptions;

public class InvalidNationalTaxIdentificationNumberException : ApplicationException
{
    public InvalidNationalTaxIdentificationNumberException(string? message) : base(message) { }
}
