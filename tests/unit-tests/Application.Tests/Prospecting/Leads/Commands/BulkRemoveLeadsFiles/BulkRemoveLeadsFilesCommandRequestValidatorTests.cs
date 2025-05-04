using Application.Prospecting.Leads.Commands.BulkRemoveLeadsFiles;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Commands.BulkRemoveLeadsFiles;

public sealed class BulkRemoveLeadsFilesCommandRequestValidatorTests
{
	private readonly BulkRemoveLeadsFilesCommandRequestValidator _sut;
	private const string NecessarioInformarAoMenos1Arquivo = "É necessário informar ao menos 1 arquivo.";
	private const string NaoPodeSerNulo = "O argumento Ids não pode ser nulo.";

	public BulkRemoveLeadsFilesCommandRequestValidatorTests()
	{
		_sut = new();
	}

	[Theory]
	[MemberData(nameof(ValidCommandRequestsSimulations))]
	public void Validate_ValidRequestParameters_ShouldSucceed(BulkRemoveLeadsFilesCommandRequest request)
	{
		//Arrange
		//Act
		var result = _sut.Validate(request);

		//Assert
		result.IsValid.Should().BeTrue();
		result.Errors.Should().BeNullOrEmpty();
	}

	[Theory]
	[MemberData(nameof(InvalidCommandRequestsSimulations))]
	public void Validate_InvalidRequestParameters_ShouldFail(
		BulkRemoveLeadsFilesCommandRequest request,
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

	public static IEnumerable<object[]> ValidCommandRequestsSimulations()
	{
		yield return new object[]
		{
			new BulkRemoveLeadsFilesCommandRequest { Ids = [new(Guid.Empty,"File1.dat")] }
		};

		yield return new object[]
		{
			new BulkRemoveLeadsFilesCommandRequest { Ids = [new(Guid.Empty,"File1.dat"), new(Guid.NewGuid(), "File2.dat")] }
		};
	}

	public static IEnumerable<object[]> InvalidCommandRequestsSimulations()
	{
		yield return new object[]
		{
			new BulkRemoveLeadsFilesCommandRequest { Ids = [] },
			NecessarioInformarAoMenos1Arquivo
		};
	}
}