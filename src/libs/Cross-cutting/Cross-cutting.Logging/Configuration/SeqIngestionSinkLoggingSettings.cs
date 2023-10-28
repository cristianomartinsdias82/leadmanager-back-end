namespace CrossCutting.Logging.Configuration;

public sealed record SeqIngestionSinkLoggingSettings : LoggingSettings
{
    public string ServerUrl { get; init; } = default!;
}