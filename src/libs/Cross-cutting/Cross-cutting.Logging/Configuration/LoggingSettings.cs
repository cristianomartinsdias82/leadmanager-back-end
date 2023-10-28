namespace CrossCutting.Logging.Configuration;

public record LoggingSettings
{
    public const string SectionName = "Logging";

    public bool Enabled { get; init; }
    public string LoggingLevel { get; init; } = default!;

    public DatabaseSinkLoggingSettings DatabaseSink { get; init; } = default!;
    public ConsoleSinkLoggingSettings ConsoleSink { get; init; } = default!;
    public FileSinkLoggingSettings FileSink { get; init; } = default!;
    public SeqIngestionSinkLoggingSettings SeqIngestionSink { get; init; } = default!;
}