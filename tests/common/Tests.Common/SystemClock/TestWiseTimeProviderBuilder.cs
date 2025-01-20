namespace Tests.Common.SystemClock;

public class TestWiseTimeProviderBuilder
{
	private int _year;
	private int _month;
	private int _day;
	private int _hour;
	private int _minute;
	private int _second;
	private int _milliSecond;
	private int _microSecond;

	private TestWiseTimeProviderBuilder() { }

	public static TestWiseTimeProviderBuilder New()
		=> new();

	public TestWiseTimeProviderBuilder WithYear(int year)
	{
		_year = year;

		return this;
	}

	public TestWiseTimeProviderBuilder WithMonth(int month)
	{
		_month = month;

		return this;
	}

	public TestWiseTimeProviderBuilder WithDay(int day)
	{
		_day = day;

		return this;
	}

	public TestWiseTimeProviderBuilder WithHour(int hour)
	{
		_hour = hour;

		return this;
	}

	public TestWiseTimeProviderBuilder WithMinute(int minute)
	{
		_minute = minute;

		return this;
	}

	public TestWiseTimeProviderBuilder WithSecond(int second)
	{
		_second = second;

		return this;
	}

	public TestWiseTimeProviderBuilder WithMilliSecond(int milliSecond)
	{
		_milliSecond = milliSecond;

		return this;
	}

	public TestWiseTimeProviderBuilder WithMicroSecond(int microSecond)
	{
		_microSecond = microSecond;

		return this;
	}

	public TimeProvider BuildTimeProvider()
		=> new TestWiseTimeProvider(
				_year,
				_month,
				_day,
				_hour,
				_minute,
				_second,
				_milliSecond,
				_microSecond);
}