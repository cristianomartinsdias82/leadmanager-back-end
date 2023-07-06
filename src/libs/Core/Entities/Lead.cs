using Core.ValueObjects;
using LanguageExt.Common;

namespace Core.Entities;

public class Lead : Entity
{
    private Result<Cnpj> Documento { get; set; }
    private Result<Endereco> Endereco { get; set; }

    public string RazaoSocial { get; private set; }
    public string Cnpj { get; private set; } = default!;

    public string Cep { get; private set; } = default!;
    public string Logradouro { get; private set; } = default!;
    public string Cidade { get; private set; } = default!;
    public string Bairro { get; private set; } = default!;
    public string Estado { get; private set; } = default!;
    public string? Numero { get; private set; }
    public string? Complemento { get; private set; }

    private Lead() { }

    public Lead(
        string cnpj,
        string razaoSocial,
        string cep,
        string endereco,
        string bairro,
        string cidade,
        string estado,
        string? numero,
        string? complemento)
    {
        Documento = ValueObjects.Cnpj.New(cnpj);
        Documento.IfFail(exc => throw exc);
        Endereco = ValueObjects.Endereco.New(cep, endereco, bairro, cidade, estado, numero, complemento);
        Endereco.IfFail(exc => throw exc);

        RazaoSocial = razaoSocial;
        Documento.IfSucc(cnpj => Cnpj = cnpj.Value!);
        Endereco.IfSucc(endereco =>
        {
            endereco.Cep.IfSucc(cep => Cep = cep);

            Logradouro = endereco.Descricao;
            Cidade = endereco.Cidade;
            Bairro = endereco.Bairro;
            Estado = endereco.Estado;
            Numero = endereco.Numero;
            Complemento = endereco.Complemento;
        });
    }

    public ValueTask<bool> Atualizar(
        string razaoSocial,
        string cnpj,
        string cep,
        string endereco,
        string cidade,
        string bairro,
        string? numero,
        string? complemento)
    {
        RazaoSocial = razaoSocial;
        Cnpj = cnpj;
        Cep = cep;
        Logradouro = endereco;
        Cidade = cidade;
        Bairro = bairro;
        Numero = numero;
        Complemento = complemento;

        return new ValueTask<bool>(true);
    }
}