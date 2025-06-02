namespace Shared.Settings;

public sealed record DataSourceSettings
(
	string ConnectionString,
	string ProviderName,
	int CommandTimeoutInSecs,
	int RetryOperationMaxCount,
	int HealthCheckingTimeoutInSecs,
	string? DatabaseName
);