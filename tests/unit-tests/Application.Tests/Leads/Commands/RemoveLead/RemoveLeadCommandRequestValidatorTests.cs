using Application.Features.Leads.Commands.RemoveLead;
using Xunit;
using FluentAssertions;

namespace Application.Tests.Leads.Commands.RemoveLead;

public sealed class RemoveLeadCommandRequestValidatorTests
{
    private readonly RemoveLeadCommandRequestValidator _validator = new();

    [Theory]
    [MemberData(nameof(InvalidCommandRequestsSimulations))]
    public void Command_InvalidRequestParameters_ShouldFail(
        RemoveLeadCommandRequest request,
        params string[] expectedErrorMessages)
    {
        //Arrange & Act
        var result = _validator.Validate(request);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeNullOrEmpty();
        result.Errors.Count.Should().Be(expectedErrorMessages.Count());
        result.Errors.Select(err => err.ErrorMessage).Should().Contain(expectedErrorMessages);
    }

    [Fact]
    public void Command_ValidRequestParameters_ShouldSucceed()
    {
        //Arrange & Act
        var result = _validator.Validate(new RemoveLeadCommandRequest { Id = Guid.NewGuid() });

        //Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeNullOrEmpty();
    }

    public static IEnumerable<object[]> InvalidCommandRequestsSimulations()
    {
        yield return new object[]
        {
            new RemoveLeadCommandRequest { Id = Guid.Empty },
            "Campo Id é obrigatório."
        };
    }
}
