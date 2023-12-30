namespace Domain.Prospecting.Exceptions;

public sealed class InvalidNationalTaxIdentificationNumberException : BusinessException
{
    public InvalidNationalTaxIdentificationNumberException(string? message) : base(message) { }
}
