using Core.ValueObjects;
using LanguageExt.Common;

namespace Core.Entities;

public class Lead : Entity
{
    private Result<Cnpj> Documento { get; set; }
    private Result<Endereco> Endereco { get; set; }

    public string RazaoSocial { get; private set; } = default!;
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
        Documento = ValidarCnpj(cnpj);
        Endereco = ValidarEndereco(cep, endereco, bairro, cidade, estado, numero, complemento);
        
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

        RazaoSocial = razaoSocial;
    }

    public void Atualizar(
        string razaoSocial,
        string cnpj,
        string cep,
        string endereco,
        string cidade,
        string estado,
        string bairro,
        string? numero,
        string? complemento)
    {
        ValidarCnpj(cnpj);
        ValidarEndereco(cep, endereco, bairro, cidade, estado, numero, complemento);

        RazaoSocial = razaoSocial;
        Cnpj = cnpj;
        Cep = cep;
        Logradouro = endereco;
        Cidade = cidade;
        Estado = estado;
        Bairro = bairro;
        Numero = numero;
        Complemento = complemento;
    }

    private Result<Cnpj> ValidarCnpj(string cnpj)
    {
        Documento = ValueObjects.Cnpj.New(cnpj);
        Documento.IfFail(exc => throw exc);

        return Documento;
    }

    private Result<Endereco> ValidarEndereco(string cep, string endereco, string bairro, string cidade, string estado, string? numero, string? complemento)
    {
        Endereco = ValueObjects.Endereco.New(cep, endereco, bairro, cidade, estado, numero, complemento);
        Endereco.IfFail(exc => throw exc);

        return Endereco;
    }
}