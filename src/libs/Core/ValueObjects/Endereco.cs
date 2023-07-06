using Core.BusinessExceptions;
using LanguageExt.Common;

namespace Core.ValueObjects;

public record Endereco
{
    protected Endereco(
        string cep,
        string descricao,
        string bairro,
        string cidade,
        string estado,
        string? numero,
        string? complemento)
    {
        Cep = cep;
        Descricao = descricao;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
        Numero = numero;
        Complemento = complemento;
    }

    public static Result<Endereco> New(string cep, string descricao, string bairro, string cidade, string estado, string? numero, string? complemento)
    {
        var cepResult = ValueObjects.Cep.New(cep);
        if (!cepResult.IsSuccess)
            return new Result<Endereco>(new InvalidAddressException("Cep inválido."));

        return new Endereco(
            cepResult.Match(ok => ok.Value, _ => default)!,
            descricao,
            bairro,
            cidade,
            estado,
            numero,
            complemento);
    }

    public Result<string> Cep { get; init; }
    public string Descricao { get; init; }
    public string Bairro { get; init; }
    public string Cidade { get; init; }
    public string Estado { get; init; }
    public string? Numero { get; init; }
    public string? Complemento { get; init; }
}