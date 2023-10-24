namespace ViaCep.ServiceClient.Configuration;

public sealed record ViaCepIntegrationSettings
{
    public int RequestTimeoutInSeconds { get; init; }
    public string Uri { get; init; } = default!;
    public string Endpoint { get; init; } = default!;
    public int HealthCheckingTimeoutInSecs { get; init; }
}