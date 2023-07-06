namespace Infrastructure.Configuration;

public record DataSourceSettings
{
    public string ConnectionString { get; init; } = default!;
    public int RetryOperationMaxCount { get; init; }
}   