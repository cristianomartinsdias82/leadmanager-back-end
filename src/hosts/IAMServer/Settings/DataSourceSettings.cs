namespace IAMServer.Settings;

public sealed record DataSourceSettings
(
    string ConnectionString,
    string DatabaseName,
    int RetryOperationMaxCount
);