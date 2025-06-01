namespace LeadManager.BackendServices.ReportGeneration.Core.Configuration;

public sealed record DataSourceSettings
(
    string ConnectionString,
    string ProviderName,
	int CommandTimeoutInSecs,
	int RetryOperationMaxCount
);