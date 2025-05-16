using Application.Prospecting.Leads.Queries.DownloadLeadsFile;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Queries.DownloadLeadsFile;

public sealed class DownloadLeadsFileQueryRequestValidatorTests
{
	private readonly DownloadLeadsFileQueryRequestValidator _sut = new();
	public DownloadLeadsFileQueryRequestValidatorTests()
	{
		_sut = new();
	}

	[Fact]
	public void Validate_ValidRequestParameters_Succeeds()
	{
		//Arrange
		//Act
		var result = _sut.Validate(new DownloadLeadsFileQueryRequest { Id = Guid.NewGuid() });

		//Assert
		result.IsValid.Should().BeTrue();
		result.Errors.Should().BeNullOrEmpty();
	}

	[Theory]
	[MemberData(nameof(InvalidQueryRequestsSimulations))]
	public void Validate_InvalidRequestParameters_Fails(
		DownloadLeadsFileQueryRequest request,
		params string[] expectedErrorMessages)
	{
		//Arrange
		//Act
		var result = _sut.Validate(request);

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
			new DownloadLeadsFileQueryRequest { Id = Guid.Empty },
			"Campo Id é obrigatório."
		};
	}
}
