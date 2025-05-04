using Domain.Core;
using Domain.Prospecting.ValueObjects;
using LanguageExt.Common;
using Shared.Generators;

namespace Domain.Prospecting.Entities;

public class Lead : Entity, ILead
{
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

    public static Lead Criar(
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
        ValidarCnpj(cnpj);
        ValidarEndereco(cep, endereco, bairro, cidade, estado, numero, complemento);

        var newId = IdGenerator.NextId();

        return new()
        {
            Id = newId,
            Cnpj = cnpj,
            RazaoSocial = razaoSocial,
            Cep = cep,
            Logradouro = endereco,
            Bairro = bairro,
            Cidade = cidade,
            Estado = estado,
            Numero = numero,
            Complemento = complemento,
            RowVersion = newId.ToByteArray()
        };
    }

    public void Atualizar(
        string razaoSocial,
        string cep,
        string endereco,
        string bairro,
        string cidade,
        string estado,
        string? numero,
        string? complemento)
    {
        ValidarEndereco(cep, endereco, bairro, cidade, estado, numero, complemento);

        RazaoSocial = razaoSocial;
        Cep = cep;
        Logradouro = endereco;
        Cidade = cidade;
        Bairro = bairro;
        Estado = estado;
        Numero = numero;
        Complemento = complemento;
    }

    public static Lead MapFromDto(LeadDto leads)
        => new()
        {
            Id = leads.Id,
            RazaoSocial = leads.RazaoSocial,
            Cnpj = leads.Cnpj,
            Logradouro = leads.Endereco,
            Numero = leads.Numero,
            Complemento = leads.Complemento,
            Bairro = leads.Bairro,
            Cep = leads.Cep,
            Cidade = leads.Cidade,
            Estado = leads.Estado,
            RowVersion = leads.Revision
        };

    //TODO: This Dto is not part of the Entities. Fix it!
    public LeadDto MapToDto()
        => new(Id, RazaoSocial, Cnpj, Logradouro, Numero, Complemento, Cep, Cidade, Bairro, Estado, RowVersion);

    private static Result<Cnpj> ValidarCnpj(string cnpj)
        => ValueObjects.Cnpj
            .New(cnpj)
            .IfFail(exc => throw exc);

    private static Result<Endereco> ValidarEndereco(string cep, string endereco, string bairro, string cidade, string estado, string? numero, string? complemento)
        => Endereco
            .New(cep, endereco, bairro, cidade, estado, numero, complemento)
            .IfFail(exc => throw exc);
}