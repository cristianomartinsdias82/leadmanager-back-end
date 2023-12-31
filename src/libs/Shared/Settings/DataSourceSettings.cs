﻿namespace Shared.Settings;

public sealed record DataSourceSettings
(
    string ConnectionString,
    int RetryOperationMaxCount,
    int HealthCheckingTimeoutInSecs,
    string? DatabaseName
);