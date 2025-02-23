using OpenTelemetry;
using System.Diagnostics;

namespace Shared.Diagnostics;

public class DiagnosticsDataCollector
{
	private Activity _activity = default!;

	private List<(string Key, object? Value)> _tags = new();
	private List<ActivityEvent> _events = new();
	private List<ActivityLink> _links = new();
	private List<(Exception Exception, TagList? Tags, DateTimeOffset Timestamp)> _exceptions = new();
	private bool _setActivityStatusAsError;
	private ActivityStatusCode? _statusCode;

	private List<(Baggage Baggage, (string Key, string? Value)[] BaggageItems)> _baggageData = new();

	private List<(Func<string, string?, Baggage> SetBaggageItem, (string Key, string? Value)[] BaggageItems)> _setBaggageItemFunc = new();

	private DiagnosticsDataCollector() { }

	public static DiagnosticsDataCollector WithActivity(Activity? activity, string activityName = "")
	{
		return new()
		{
			_activity = activity ?? new Activity(activityName ?? string.Empty)
		};
	}

	public static DiagnosticsDataCollector WithActivityFromSource(
		ActivitySource source,
		string name,
		ActivityKind kind,
		ActivityContext? context,
		IEnumerable<ActivityLink>? links = null)
	{
		var activity = source
						.StartActivity(
							name,
							kind,
							context ?? new ActivityContext(),
							links: links);

		ArgumentNullException.ThrowIfNull(activity);

		return new()
		{
			_activity = activity
		};
	}

	public Activity Activity { get { return _activity; } }

	public DiagnosticsDataCollector WithTags(params (string Key, object? Value)[] tags)
	{
		_tags.AddRange(tags);

		return this;
	}

	public DiagnosticsDataCollector WithTag((string Key, object? Value) tag)
	{
		_tags.Add(tag);

		return this;
	}

	public DiagnosticsDataCollector WithEvent(
		string name,
		DateTimeOffset timestamp,
		params (string Key, object Value)[] tags)
	{
		_events.Add(new(
			name,
			timestamp,
			tags: new ActivityTagsCollection([..tags.Select(tag => new KeyValuePair<string, object?>(tag.Key, tag.Value))])));

		return this;
	}

	public DiagnosticsDataCollector WithStatus(ActivityStatusCode statusCode)
	{
		_statusCode = statusCode;

		return this;
	}

	public DiagnosticsDataCollector WithLink(ActivityContext activityContext, ActivityTagsCollection? tags = default)
	{
		_links.Add(new(activityContext, tags));

		return this;
	}

	public DiagnosticsDataCollector WithException(Exception exception, DateTimeOffset timestamp, bool setStatusAsError = true, TagList? tags = default)
	{
		_exceptions.Add((exception, tags, timestamp));

		_setActivityStatusAsError = setStatusAsError;

		return this;
	}

	public DiagnosticsDataCollector WithBaggageData(Baggage baggage, params (string Key, string? Value)[] baggageItems)
	{
		_baggageData.Add((baggage, baggageItems));

		return this;
	}

	public DiagnosticsDataCollector WithBaggageData(Func<string, string?, Baggage> setBaggageItem, params (string Key, string? Value)[] baggageItems)
	{
		_setBaggageItemFunc.Add((setBaggageItem, baggageItems));

		return this;
	}

	public ValueTask PushData()
	{
		if (_statusCode is not null)
			_activity.SetStatus(_statusCode.Value);

		foreach (var tag in _tags)
			_activity.SetTag(tag.Key, tag.Value);

		foreach (var @event in _events)
			_activity.AddEvent(@event);

		foreach (var link in _links)
			_activity.AddLink(link);

		foreach (var exception in _exceptions)
		{
			_activity.AddException(exception.Exception, exception.Tags ?? default, exception.Timestamp);

			if (_setActivityStatusAsError)
				_activity.SetStatus(ActivityStatusCode.Error);
		}

		//foreach (var baggageData in _baggageData)
		//	baggageData.BaggageItems
		//				.ToList()
		//				.ForEach(bi => baggageData.Baggage.SetBaggage(bi.Key, bi.Value));

		foreach (var baggageData in _setBaggageItemFunc)
			baggageData.BaggageItems
						.ToList()
						.ForEach(bi => baggageData.SetBaggageItem(bi.Key, bi.Value));

		_activity.Dispose();

		return ValueTask.CompletedTask;
	}
}
