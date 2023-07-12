using Application.Features.Leads.Commands.RegisterLead;
using FluentAssertions;
using Tests.Common.ObjectMothers.Application;
using Xunit;

namespace Application.Tests.Leads.Commands.RegisterLead;

public sealed class RegisterLeadCommandRequestValidatorTests
{
    private readonly RegisterLeadCommandRequestValidator _validator = new();

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
        var emptyDataRequest = RegisterLeadCommandRequestMother.Instance.Build();

        yield return new object[]
        {
            emptyDataRequest,
            "Campo Cnpj é obrigatório.",
            "Campo Cep é obrigatório.",
            "Campo Endereço é obrigatório.",
            "Campo Bairro é obrigatório.",
            "Campo Cidade é obrigatório.",
            "Campo Estado é obrigatório."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother
                .Instance
                .WithRazaoSocial("XptoInc")
                .Build(),
            "Campo Cnpj é obrigatório.",
            "Campo Cep é obrigatório.",
            "Campo Endereço é obrigatório.",
            "Campo Bairro é obrigatório.",
            "Campo Cidade é obrigatório.",
            "Campo Estado é obrigatório."
        };

        yield return new object[]
        {
            RegisterLeadCommandRequestMother
                .Instance
                .WithCnpj("21.456.987/0005-54")
                .WithRazaoSocial("XptoInc")
                .Build(),
            "Campo Cep é obrigatório.",
            "Campo Cnpj é inválido.",
            "Campo Endereço é obrigatório.",
            "Campo Bairro é obrigatório.",
            "Campo Cidade é obrigatório.",
            "Campo Estado é obrigatório."
        };

        /*
        
        ...
         
        */

        var xptoIncWithInvalidState = RegisterLeadCommandRequestMother.XptoIncLeadRequest();
        xptoIncWithInvalidState.Estado = "SPA";

        yield return new object[]
        {
            xptoIncWithInvalidState,
            "Campo Estado deve conter 2 caracteres."
        };
    }
}