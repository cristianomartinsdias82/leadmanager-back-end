namespace CrossCutting.Security.Authentication.JsonWebTokens.Configuration;

public sealed record JwtAuthenticationProviderSettings
(
    string AuthorityBaseUri,
    string Audience,
    int HealthCheckingTimeoutInSecs
);
