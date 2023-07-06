using Core.BusinessExceptions;
using LanguageExt.Common;
using System.Text.RegularExpressions;

namespace Core.ValueObjects;

public class Cnpj
{
    public string? Value { get; private set; }

    protected Cnpj() { }

    public static Result<Cnpj> New(string cnpj)
    {
        if (!Regex.IsMatch(cnpj, @"[0-9]{2}\.[0-9]{3}\.[0-9]{3}\/[0-9]{4}\-[0-9]{2}"))
            return new Result<Cnpj>(new InvalidNationalTaxIdentificationNumberException("Cnpj inválido."));

        return new Result<Cnpj>(new Cnpj() { Value = cnpj });
    }
}