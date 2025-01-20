namespace Tests.Common.SystemClock;

internal sealed class TestWiseTimeProvider : TimeProvider
{
	private DateTimeOffset _dateTimeOffset;

	public TestWiseTimeProvider(
		int hour,
		int minute,
		int second = 0,
		int millisecond = 0,
		int microsecond = 0)
	{
		var now = GetUtcNow();
		_dateTimeOffset = new(
							now.Year,
							now.Month,
							now.Day,
							hour,
							minute,
							second,
							millisecond,
							microsecond,
							TimeSpan.Zero);
	}

	public TestWiseTimeProvider(
		int year,
		int month,
		int day,
		int hour,
		int minute,
		int second = 0,
		int millisecond = 0,
		int microsecond = 0)
	{
		_dateTimeOffset = new(
							year,
							month,
							day,
							hour,
							minute,
							second,
							millisecond,
							microsecond,
							TimeSpan.Zero);
	}

	public override DateTimeOffset GetUtcNow() => _dateTimeOffset;
}