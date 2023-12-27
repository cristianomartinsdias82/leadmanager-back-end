using Application.Security.OneTimePassword.Commands.GenerateOneTimePassword;
using FluentAssertions;
using Xunit;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace Application.Tests.Security.OneTimePassword.Commands.GenerateOneTimePassword;

public sealed class GenerateOneTimePasswordCommandRequestValidatorTests
{
    private readonly GenerateOneTimePasswordCommandRequestValidator _validator = new();

    //[Fact]
    [Theory]
    [InlineData(Claims.Read)]
    [InlineData(Claims.Insert)]
    [InlineData(Claims.BulkInsert)]
    [InlineData(Claims.Update)]
    [InlineData(Claims.Delete)]
    public void Validate_ValidRequestParameters_ShouldSucceed(string resource)
    {
        //Arrange
        //Act
        var result = _validator.Validate(new GenerateOneTimePasswordCommandRequest() { Resource = resource });

        //Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeNullOrEmpty();
    }

    [Theory]
    [MemberData(nameof(InvalidQueryRequestsSimulations))]
    public void Validate_InvalidRequestParameters_ShouldFail(GenerateOneTimePasswordCommandRequest request, string expectedErrorMessage)
    {
        //Arrange
        //Act
        var result = _validator.Validate(request);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeNullOrEmpty();
        result.Errors.Select(err => err.ErrorMessage).Should().Contain(expectedErrorMessage);
    }

    public static IEnumerable<object[]> InvalidQueryRequestsSimulations()
    {
        yield return new object[]
        {
            new GenerateOneTimePasswordCommandRequest { Resource = null! },
            "Campo Resource é obrigatório."
        };

        yield return new object[]
        {
            new GenerateOneTimePasswordCommandRequest { Resource = string.Empty },
            "Campo Resource é obrigatório."
        };

        yield return new object[]
        {
            new GenerateOneTimePasswordCommandRequest { Resource = " " },
            "Campo Resource é obrigatório."
        };

        yield return new object[]
        {
            new GenerateOneTimePasswordCommandRequest { Resource = "X" },
            "Campo Resource é inválido."
        };
    }
}