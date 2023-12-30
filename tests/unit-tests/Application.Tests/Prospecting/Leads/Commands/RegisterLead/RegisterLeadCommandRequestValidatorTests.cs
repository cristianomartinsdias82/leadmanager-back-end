using Application.Prospecting.Leads.Commands.RegisterLead;
using FluentAssertions;
using Tests.Common.ObjectMothers.Application;
using Tests.Common.ObjectMothers.Domain;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Commands.RegisterLead;

public sealed class RegisterLeadCommandRequestValidatorTests
{
    private readonly RegisterLeadCommandRequestValidator _validator = new();
    private static readonly RegisterLeadCommandRequest _validLeadRequest = RegisterLeadCommandRequestMother.XptoIncLeadRequest();

    [Theory]
    [MemberData(nameof(ValidCommandRequestsSimulations))]
    public void Validate_ValidRequestParameters_ShouldSucceed(RegisterLeadCommandRequest request)
    {
        //Arrange
        //Act
        var result = _validator.Validate(request);

        //Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeNullOrEmpty();
    }

    [Theory]
    [MemberData(nameof(InvalidCommandRequestsSimulations))]
    public void Validate_InvalidRequestParameters_ShouldFail(
        RegisterLeadCommandRequest request,
        params string[] expectedErrorMessages)
    {
        //Arrange
        //Act
        var result = _validator.Validate(request);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeNullOrEmpty();
        result.Errors.Count.Should().Be(expectedErrorMessages.Count());
        result.Errors.Select(err => err.ErrorMessage).Should().Contain(expectedErrorMessages);
    }

    public static IEnumerable<object[]> ValidCommandRequestsSimulations()
    {
        var xptoInc = RegisterLeadCommandRequestMother.XptoIncLeadRequest();

        yield return new object[]
        {
            xptoInc
        };
    }

    public static IEnumerable<object[]> InvalidCommandRequestsSimulations()
    {
        yield return new object[]
        {
            RegisterLeadCommandRequestMother.Instance
                .Build(),
            "Campo Cnpj é obrigatório.",
            "Campo Razão social é obrigatório.",
            "Campo Cep é obrigatório.",
            "Campo Endereço é obrigatório.",
            "Campo Bairro é obrigatório.",
            "Campo Cidade é obrigatório.",
            "Campo Estado é obrigatório."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother.Instance
                .WithCnpj(CnpjMother.MaskedWellformedValidOne())
                .Build(),
            "Campo Razão social é obrigatório.",
            "Campo Cep é obrigatório.",
            "Campo Endereço é obrigatório.",
            "Campo Bairro é obrigatório.",
            "Campo Cidade é obrigatório.",
            "Campo Estado é obrigatório."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother.Instance
                .WithCnpj(CnpjMother.MaskedMalformedValidOne())
                .Build(),
            "Campo Cnpj é inválido.",
            "Campo Razão social é obrigatório.",
            "Campo Cep é obrigatório.",
            "Campo Endereço é obrigatório.",
            "Campo Bairro é obrigatório.",
            "Campo Cidade é obrigatório.",
            "Campo Estado é obrigatório."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother.Instance
                .WithCnpj(CnpjMother.UnmaskedValidOne())
                .Build(),
            "Campo Cnpj é inválido.",
            "Campo Razão social é obrigatório.",
            "Campo Cep é obrigatório.",
            "Campo Endereço é obrigatório.",
            "Campo Bairro é obrigatório.",
            "Campo Cidade é obrigatório.",
            "Campo Estado é obrigatório."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother.Instance
                .WithCnpj(CnpjMother.UnmaskedInvalidOne())
                .Build(),
            "Campo Cnpj é inválido.",
            "Campo Razão social é obrigatório.",
            "Campo Cep é obrigatório.",
            "Campo Endereço é obrigatório.",
            "Campo Bairro é obrigatório.",
            "Campo Cidade é obrigatório.",
            "Campo Estado é obrigatório."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother.Instance
                .WithCnpj(CnpjMother.MaskedWellformedValidOne())
                .WithRazaoSocial(_validLeadRequest.RazaoSocial!)
                .Build(),
            "Campo Cep é obrigatório.",
            "Campo Endereço é obrigatório.",
            "Campo Bairro é obrigatório.",
            "Campo Cidade é obrigatório.",
            "Campo Estado é obrigatório."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother.Instance
                .WithCnpj(CnpjMother.MaskedWellformedValidOne())
                .WithRazaoSocial(_validLeadRequest.RazaoSocial!)
                .WithCep(CepMother.MaskedWellformedValidOne())
                .Build(),
            "Campo Endereço é obrigatório.",
            "Campo Bairro é obrigatório.",
            "Campo Cidade é obrigatório.",
            "Campo Estado é obrigatório."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother.Instance
                .WithCnpj(CnpjMother.MaskedWellformedValidOne())
                .WithRazaoSocial("Xpto Inc.")
                .WithCep(CepMother.MaskedWellformedValidOne())
                .WithEndereco("Rua Xpto")
                .Build(),
            "Campo Bairro é obrigatório.",
            "Campo Cidade é obrigatório.",
            "Campo Estado é obrigatório."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother.Instance
                .WithCnpj(CnpjMother.MaskedWellformedValidOne())
                .WithRazaoSocial(_validLeadRequest.RazaoSocial!)
                .WithCep(CepMother.MaskedWellformedValidOne())
                .WithEndereco("Rua Xpto")
                .WithBairro(_validLeadRequest.Bairro!)
                .Build(),
            "Campo Cidade é obrigatório.",
            "Campo Estado é obrigatório."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother.Instance
                .WithCnpj(CnpjMother.MaskedWellformedValidOne())
                .WithRazaoSocial(_validLeadRequest.RazaoSocial!)
                .WithCep(CepMother.MaskedWellformedValidOne())
                .WithEndereco(_validLeadRequest.Endereco!)
                .WithBairro(_validLeadRequest.Bairro!)
                .WithCidade(_validLeadRequest.Cidade!)
                .Build(),
            "Campo Estado é obrigatório."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother.Instance
                .WithCnpj(CnpjMother.MaskedWellformedValidOne())
                .WithRazaoSocial(_validLeadRequest.RazaoSocial!)
                .WithCep(CepMother.MaskedWellformedValidOne())
                .WithEndereco(_validLeadRequest.Endereco!)
                .WithBairro(_validLeadRequest.Bairro!)
                .WithCidade(_validLeadRequest.Cidade!)
                .WithEstado(string.Empty)
                .Build(),
            "Campo Estado é obrigatório.",
            "Campo Estado deve conter 2 caracteres."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother.Instance
                .WithCnpj(CnpjMother.MaskedWellformedValidOne())
                .WithRazaoSocial(_validLeadRequest.RazaoSocial!)
                .WithCep(CepMother.MaskedWellformedValidOne())
                .WithEndereco(_validLeadRequest.Endereco!)
                .WithBairro(_validLeadRequest.Bairro!)
                .WithCidade(_validLeadRequest.Cidade!)
                .WithEstado("S")
                .Build(),
            "Campo Estado deve conter 2 caracteres."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother.Instance
                .WithCnpj(CnpjMother.MaskedWellformedValidOne())
                .WithRazaoSocial(_validLeadRequest.RazaoSocial!)
                .WithCep(CepMother.MaskedWellformedValidOne())
                .WithEndereco(_validLeadRequest.Endereco!)
                .WithBairro(_validLeadRequest.Bairro!)
                .WithCidade(_validLeadRequest.Cidade!)
                .WithEstado("SPx")
                .Build(),
            "Campo Estado deve conter 2 caracteres."
        };
    }
}