namespace ViaCep.ServiceClient.Configuration;

public sealed record ViaCepIntegrationSettings
(
    int RequestTimeoutInSeconds,
    string Uri,
    string Endpoint
);