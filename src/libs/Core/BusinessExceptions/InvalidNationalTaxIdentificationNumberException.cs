namespace Core.BusinessExceptions;

public sealed class InvalidNationalTaxIdentificationNumberException : BusinessException
{
    public InvalidNationalTaxIdentificationNumberException(string? message) : base(message) { }
}
