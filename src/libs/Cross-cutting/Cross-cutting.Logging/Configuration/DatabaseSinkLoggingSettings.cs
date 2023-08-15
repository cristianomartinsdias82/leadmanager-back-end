namespace CrossCutting.Logging.Configuration;

public sealed record DatabaseSinkLoggingSettings : LoggingSettings
{
    public string LogTableName { get; init; } = default!;
    public string LogSchemaName { get; init; } = default!;
    public bool AutoCreateTable { get; init; } = default!;
}
