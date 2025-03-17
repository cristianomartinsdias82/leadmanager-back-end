using Application.Core.OperatingRules;
using FluentAssertions;
using Tests.Common.ObjectMothers.DateTimeHandling;
using Xunit;

namespace Application.Tests.Core.OperatingRules;

public sealed class WeekDaysOnlyOperatingRuleTests
{
	[Fact]
	public async Task Apply_Weekends_ProducesRuleViolation()
	{
		//Arrange
		var timeProvider = TimeProviderMother.Saturday();
		var sut = new WeekDaysOnlyOperatingRule(timeProvider);

		//Act
		var result = await sut.ApplyAsync();

		//Assert
		result.Should().NotBeNull();
	}

	[Fact]
	public async Task Apply_BusinessDays_DoesNotProduceRuleViolation()
	{
		//Arrange
		var timeProvider = TimeProviderMother.MondayInBusinessHoursTimeWindowMorning();
		var sut = new WeekDaysOnlyOperatingRule(timeProvider);

		//Act
		var result = await sut.ApplyAsync();

		//Assert
		result.Should().BeNull();
	}
}
