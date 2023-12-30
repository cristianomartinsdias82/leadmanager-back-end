using Application.Prospecting.Leads.Queries.SearchLead;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Queries.SearchLead;

public sealed class GetLeadByIdRequestValidatorTests
{
    private readonly SearchLeadQueryRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequestParameters_ShouldSucceed()
    {
        //Arrange
        //Act
        var result = _validator.Validate(new SearchLeadQueryRequest(default, "Gumper Inc."));

        //Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeNullOrEmpty();
    }

    [Theory]
    [MemberData(nameof(InvalidQueryRequestsSimulations))]
    public void Validate_InvalidRequestParameters_ShouldFail(
        SearchLeadQueryRequest request,
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
            new SearchLeadQueryRequest(default, null!),
            "Termo para pesquisa é obrigatório."
        };

        yield return new object[]
        {
            new SearchLeadQueryRequest(default, ""),
            "Termo para pesquisa é obrigatório."
        };

        yield return new object[]
        {
            new SearchLeadQueryRequest(default, " "),
            "Termo para pesquisa é obrigatório."
        };
    }
}