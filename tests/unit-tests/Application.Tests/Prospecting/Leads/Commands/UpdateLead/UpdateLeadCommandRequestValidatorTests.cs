using Application.Prospecting.Leads.Commands.UpdateLead;
using FluentAssertions;
using Tests.Common.ObjectMothers.Application;
using Tests.Common.ObjectMothers.Domain;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Commands.UpdateLead;

public sealed class UpdateLeadCommandRequestValidatorTests
{
    private readonly UpdateLeadCommandRequestValidator _validator = new();
    private static readonly UpdateLeadCommandRequest _validLeadRequest = UpdateLeadCommandRequestMother.XptoIncLeadRequest();

    [Fact]
    public void Validate_ValidRequestParameters_ShouldSucceed()
    {
        //Arrange
        //Act
        var result = _validator.Validate(_validLeadRequest);

        //Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeNullOrEmpty();
    }

    [Theory]
    [MemberData(nameof(InvalidCommandRequestsSimulations))]
    public void Validate_InvalidRequestParameters_ShouldFail(
        UpdateLeadCommandRequest request,
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

    public static IEnumerable<object[]> InvalidCommandRequestsSimulations()
    {
        yield return new object[]
        {
            UpdateLeadCommandRequestMother.Instance
                .Build(),
            "Campo Id é obrigatório.",
            "Campo Razão social é obrigatório.",
            "Campo Cep é obrigatório.",
            "Campo Endereço é obrigatório.",
            "Campo Bairro é obrigatório.",
            "Campo Cidade é obrigatório.",
            "Campo Estado é obrigatório."
        };

        yield return new object[]
        {
            UpdateLeadCommandRequestMother.Instance
                .WithId()
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
            UpdateLeadCommandRequestMother.Instance
                .WithId()
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
            UpdateLeadCommandRequestMother.Instance
                .WithId()
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
            UpdateLeadCommandRequestMother.Instance
                .WithId()
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
            UpdateLeadCommandRequestMother.Instance
                .WithId()
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
            UpdateLeadCommandRequestMother.Instance
                .WithId()
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
            UpdateLeadCommandRequestMother.Instance
                .WithId()
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
            UpdateLeadCommandRequestMother.Instance
                .WithId()
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
            UpdateLeadCommandRequestMother.Instance
                .WithId()
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
            UpdateLeadCommandRequestMother.Instance
                .WithId()
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
            UpdateLeadCommandRequestMother.Instance
                .WithId()
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
            UpdateLeadCommandRequestMother.Instance
                .WithId()
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
            UpdateLeadCommandRequestMother.Instance
                .WithId()
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