using Tests.Common.SystemClock;

namespace Tests.Common.ObjectMothers.DateTimeHandling;

public static class TimeProviderMother
{
	private static TestWiseTimeProviderBuilder _timeProviderBuilder = TestWiseTimeProviderBuilder.New();

	public static TimeProvider Saturday()
		=> _timeProviderBuilder.WithYear(2025)
							   .WithMonth(01)
							   .WithDay(04)
							   .BuildTimeProvider();

	public static TimeProvider Sunday()
		=> _timeProviderBuilder.WithYear(2025)
							   .WithMonth(01)
							   .WithDay(05)
							   .BuildTimeProvider();

	public static TimeProvider MondayInBusinessHoursTimeWindowMorning()
		=> _timeProviderBuilder.WithYear(2025)
							   .WithMonth(01)
							   .WithDay(06)
							   .WithHour(10)
							   .BuildTimeProvider();

	public static TimeProvider MondayInBusinessHoursTimeWindowNoon()
		=> _timeProviderBuilder.WithYear(2025)
							   .WithMonth(01)
							   .WithDay(06)
							   .WithHour(15)
							   .BuildTimeProvider();

	public static TimeProvider MondayBeforeBusinessHoursTimeWindow()
		=> _timeProviderBuilder.WithYear(2025)
							   .WithMonth(01)
							   .WithDay(06)
							   .WithHour(05)
							   .BuildTimeProvider();

	public static TimeProvider MondayAfterBusinessHoursTimeWindow()
		=> _timeProviderBuilder.WithYear(2025)
							   .WithMonth(01)
							   .WithDay(06)
							   .WithHour(20)
							   .BuildTimeProvider();
}