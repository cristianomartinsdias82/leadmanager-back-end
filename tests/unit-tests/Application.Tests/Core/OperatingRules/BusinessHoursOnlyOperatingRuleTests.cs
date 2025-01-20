using Application.Core.OperatingRules;
using FluentAssertions;
using Tests.Common.ObjectMothers.DateTimeHandling;
using Xunit;

namespace Application.Tests.Core.OperatingRules;

public sealed class BusinessHoursOnlyOperatingRuleTests
{
	[Fact]
	public async Task Apply_InBusinessHoursRange_ShouldNotReturnAnyNonNullRuleViolation()
	{
		//Arrange
		var timeProvider = TimeProviderMother.MondayInBusinessHoursTimeWindowMorning();
		var sut = new BusinessHoursOnlyOperatingRule(timeProvider);

		//Act
		var result = await sut.ApplyAsync();
		
		//Assert
		result.Should().BeNull();
	}

	[Fact]
	public async Task Apply_BeforeBusinessHoursRange_ShouldReturnRuleViolation()
	{
		//Arrange
		var timeProvider = TimeProviderMother.MondayBeforeBusinessHoursTimeWindow();
		var sut = new BusinessHoursOnlyOperatingRule(timeProvider);

		//Act
		var result = await sut.ApplyAsync();

		//Assert
		result.Should().NotBeNull();
	}

	[Fact]
	public async Task Apply_AfterBusinessHoursRange_ShouldReturnRuleViolation()
	{
		//Arrange
		var timeProvider = TimeProviderMother.MondayBeforeBusinessHoursTimeWindow();
		var sut = new BusinessHoursOnlyOperatingRule(timeProvider);

		//Act
		var result = await sut.ApplyAsync();

		//Assert
		result.Should().NotBeNull();
	}
}