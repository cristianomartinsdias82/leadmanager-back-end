using LanguageExt.Common;
using System.Text.RegularExpressions;

namespace Domain.Prospecting.ValueObjects;

public class Cep
{
    public string? Value { get; private set; }

    protected Cep() { }

    public static Result<Cep> New(string cep)
    {
        if (!Regex.IsMatch(cep, @"[0-9]{5}\-[0-9]{3}"))
            return new Result<Cep>(new ArgumentException("Cep inválido."));

        return new Result<Cep>(new Cep() { Value = cep });
    }
}