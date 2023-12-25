using Application.Security.OneTimePassword.Commands.GenerateOneTimePassword;
using Application.Security.OneTimePassword.Commands.HandleOneTimePassword;
using FluentAssertions;
using Xunit;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace Application.Tests.Prospecting.Security.OneTimePassword.Commands.HandleOneTimePassword;

public sealed class HandleOneTimePasswordCommandRequestValidatorTests
{
    private readonly HandleOneTimePasswordCommandRequestValidator _validator = new();

    [Theory]
    [MemberData(nameof(ValidQueryRequestsSimulations))]
    public void Validate_ValidRequestParameters_ShouldSucceed(HandleOneTimePasswordCommandRequest request)
    {
        //Arrange
        //Act
        var result = _validator.Validate(request);

        //Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeNullOrEmpty();
    }

    [Theory]
    [MemberData(nameof(InvalidQueryRequestsSimulations))]
    public void Validate_InvalidRequestParameters_ShouldFail(HandleOneTimePasswordCommandRequest request, params string[] expectedErrorMessages)
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

    public static IEnumerable<object[]> ValidQueryRequestsSimulations()
    {
        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = Claims.Read, UserId = Guid.NewGuid() },
        };

        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = Claims.Insert, UserId = Guid.NewGuid() },
        };

        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = Claims.BulkInsert, UserId = Guid.NewGuid() },
        };

        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = Claims.Update, UserId = Guid.NewGuid() },
        };

        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = Claims.Delete, UserId = Guid.NewGuid() }
        };
    }


    public static IEnumerable<object[]> InvalidQueryRequestsSimulations()
    {
        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = null!, UserId = Guid.NewGuid() },
            "Campo Resource é obrigatório."
        };

        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = string.Empty, UserId = Guid.NewGuid() },
            "Campo Resource é obrigatório."
        };

        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = " ", UserId = Guid.NewGuid() },
            "Campo Resource é obrigatório."
        };

        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = "X", UserId = Guid.NewGuid() },
            "Campo Resource é inválido."
        };

        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = Claims.Read, UserId = default },
            "Id do usuário é inválido."
        };

        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = null!, UserId = default },
            "Campo Resource é obrigatório.",
            "Id do usuário é inválido."
        };

        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = string.Empty, UserId = default },
            "Campo Resource é obrigatório.",
            "Id do usuário é inválido."
        };

        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = " ", UserId = default },
            "Campo Resource é obrigatório.",
            "Id do usuário é inválido."
        };

        yield return new object[]
        {
            new HandleOneTimePasswordCommandRequest { Resource = "X", UserId = default },
            "Campo Resource é inválido.",
            "Id do usuário é inválido."
        };
    }
}