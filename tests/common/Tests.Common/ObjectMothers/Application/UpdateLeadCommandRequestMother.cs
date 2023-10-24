using Application.Prospecting.Leads.Commands.UpdateLead;

namespace Tests.Common.ObjectMothers.Application;

public class UpdateLeadCommandRequestMother
{
    private UpdateLeadCommandRequestMother()
    {
    }

    private UpdateLeadCommandRequest _request = new();

    public static UpdateLeadCommandRequestMother Instance
        => new UpdateLeadCommandRequestMother();

    public UpdateLeadCommandRequestMother WithId(Guid? guid = default)
    {
        _request.Id = guid ?? Guid.NewGuid();
        return this;
    }

    public UpdateLeadCommandRequestMother WithRazaoSocial(string razaoSocial)
    {
        _request.RazaoSocial = razaoSocial;
        return this;
    }

    public UpdateLeadCommandRequestMother WithCep(string cep)
    {
        _request.Cep = cep;
        return this;
    }

    public UpdateLeadCommandRequestMother WithEndereco(string endereco)
    {
        _request.Endereco = endereco;
        return this;
    }

    public UpdateLeadCommandRequestMother WithBairro(string bairro)
    {
        _request.Bairro = bairro;
        return this;
    }

    public UpdateLeadCommandRequestMother WithCidade(string cidade)
    {
        _request.Cidade = cidade;
        return this;
    }

    public UpdateLeadCommandRequestMother WithEstado(string estado)
    {
        _request.Estado = estado;
        return this;
    }

    public UpdateLeadCommandRequestMother WithNumero(string numero)
    {
        _request.Numero = numero;
        return this;
    }

    public UpdateLeadCommandRequestMother WithComplemento(string complemento)
    {
        _request.Complemento = complemento;
        return this;
    }

    public UpdateLeadCommandRequestMother WithRowVersion(byte[] rowVersion)
    {
        _request.Revision = rowVersion;
        return this;
    }

    public UpdateLeadCommandRequest Build()
        => _request;

    public static UpdateLeadCommandRequest XptoIncLeadRequest()
    {
        var xptoInc = RegisterLeadCommandRequestMother.XptoIncLeadRequest();
        return Instance
            .WithId()
            .WithRazaoSocial(xptoInc.RazaoSocial!)
            .WithCep(xptoInc.Cep!)
            .WithEndereco(xptoInc.Endereco!)
            .WithBairro(xptoInc.Bairro!)
            .WithCidade(xptoInc.Cidade!)
            .WithEstado(xptoInc.Estado!)
            .WithNumero(xptoInc.Numero!)
            .WithComplemento(xptoInc.Complemento!)
            .Build();
    }
}