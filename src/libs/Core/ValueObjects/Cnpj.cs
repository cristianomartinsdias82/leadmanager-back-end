using Core.BusinessExceptions;
using LanguageExt.Common;
using Shared.Validation;

namespace Core.ValueObjects;

public class Cnpj
{
    public string? Value { get; private set; }

    protected Cnpj() { }

    public static Result<Cnpj> New(string cnpj)
    {
        if (!CnpjValidator.IsValidCnpj(cnpj))
            return new Result<Cnpj>(new InvalidNationalTaxIdentificationNumberException($"O Cnpj {cnpj} é inválido."));

        return new Result<Cnpj>(new Cnpj() { Value = cnpj });
    }
}