using Application.Prospecting.Leads.Queries.GetLeadById;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Queries.GetLeadById;

public sealed class GetLeadByIdQueryRequestValidatorTests
{
    private readonly GetLeadByIdQueryRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequestParameters_ShouldSucceed()
    {
        //Arrange
        //Act
        var result = _validator.Validate(new GetLeadByIdQueryRequest() { Id = Guid.NewGuid() });

        //Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeNullOrEmpty();
    }

    [Theory]
    [MemberData(nameof(InvalidQueryRequestsSimulations))]
    public void Validate_InvalidRequestParameters_ShouldFail(
        GetLeadByIdQueryRequest request,
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

    public static IEnumerable<object[]> InvalidQueryRequestsSimulations()
    {
        yield return new object[]
        {
            new GetLeadByIdQueryRequest { Id = Guid.Empty },
            "Campo Id é obrigatório."
        };
    }
}