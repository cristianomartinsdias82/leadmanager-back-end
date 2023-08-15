namespace CrossCutting.Logging.Configuration;

public sealed record FileSinkLoggingSettings : LoggingSettings
{
    public string LogFilePath { get; init; } = default!;
}