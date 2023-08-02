namespace ViaCep.ServiceClient;

public sealed record ViaCepIntegrationSettings
(
    int RequestTimeoutInSeconds,
    string Uri,
    string Endpoint
);