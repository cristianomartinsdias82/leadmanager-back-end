using Application.Prospecting.Leads.Commands.RegisterLead;

namespace Tests.Common.ObjectMothers.Application;

public class RegisterLeadCommandRequestMother
{
    private RegisterLeadCommandRequestMother() { }

    private RegisterLeadCommandRequest _request = new();

    public static RegisterLeadCommandRequestMother Instance
        => new RegisterLeadCommandRequestMother();

    public RegisterLeadCommandRequestMother WithCnpj(string cnpj)
    {
        _request.Cnpj = cnpj;
        return this;
    }

    public RegisterLeadCommandRequestMother WithRazaoSocial(string razaoSocial)
    {
        _request.RazaoSocial = razaoSocial;
        return this;
    }

    public RegisterLeadCommandRequestMother WithCep(string cep)
    {
        _request.Cep = cep;
        return this;
    }

    public RegisterLeadCommandRequestMother WithEndereco(string endereco)
    {
        _request.Endereco = endereco;
        return this;
    }

    public RegisterLeadCommandRequestMother WithBairro(string bairro)
    {
        _request.Bairro = bairro;
        return this;
    }

    public RegisterLeadCommandRequestMother WithCidade(string cidade)
    {
        _request.Cidade = cidade;
        return this;
    }

    public RegisterLeadCommandRequestMother WithEstado(string estado)
    {
        _request.Estado = estado;
        return this;
    }

    public RegisterLeadCommandRequestMother WithNumero(string numero)
    {
        _request.Numero = numero;
        return this;
    }

    public RegisterLeadCommandRequestMother WithComplemento(string complemento)
    {
        _request.Complemento = complemento;
        return this;
    }

    public RegisterLeadCommandRequest Build()
        => _request;

    public static RegisterLeadCommandRequest XptoIncLeadRequest()
    => Instance
            .WithCnpj("60.346.523/0001-16")
            .WithRazaoSocial("Xpto Inc. LLC")
            .WithCep("01234-567")
            .WithEndereco("Rua das Pitombeiras")
            .WithBairro("Vila Alexandria")
            .WithCidade("São Paulo")
            .WithEstado("SP")
            .WithNumero("287")
            .WithComplemento("Bloco C")
            .Build();
}
