using Microsoft.Extensions.Logging;
using OpenTelemetry;
using System.Diagnostics;

namespace CrossCutting.Messaging.Consumers.BackendServicesCommon.Diagnostics.SpanProcessors;

/// <summary>
/// Think this class as a Asp.Net Middleware: it is a way to centralize logic that needs to run for every span that occurs in the application journey
/// This class adds tags based on the Baggage items
/// Refer to Program.cs to see how this Span processor is registered
/// </summary>
public sealed class ImportBaggageEntriesSpanProcessor : BaseProcessor<Activity>
{
	private readonly ILogger<ImportBaggageEntriesSpanProcessor> _logger;

	public ImportBaggageEntriesSpanProcessor(ILogger<ImportBaggageEntriesSpanProcessor> logger)
	{
		_logger = logger;
	}

	public override void OnStart(Activity data)
		=> base.OnStart(data);

	public override void OnEnd(Activity data)
	{
		_logger.LogInformation("(ImportBaggageEntriesSpanProcessor.OnEnd) Baggage item count: {BaggageEntryCount}", data.Baggage.Count());
		_logger.LogInformation("(ImportBaggageEntriesSpanProcessor.OnEnd) The following entries were found in Baggage: {BaggageEntries}", string.Join(",", data.Baggage.Select(entry => $"{entry.Key}: {entry.Value}")));

		foreach (var kvp in data.Baggage/*.Where(it => it.Key.StartsWith("client."))) //Tip: be selective to prevent undesired baggage items from ending up in http headers, 'cause this is how it works by default*/)
			data.SetTag(kvp.Key, $"(Extracted from Baggage) {kvp.Value}");

		//data.SetTag("Extra!", $"Some data added in a centralized way by the {typeof(ImportBaggageEntriesSpanProcessor).FullName!} span processor");

		base.OnEnd(data);
	}
}