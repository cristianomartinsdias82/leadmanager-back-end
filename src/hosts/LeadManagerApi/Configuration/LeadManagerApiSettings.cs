namespace LeadManagerApi.Configuration;

public sealed record LeadManagerApiSettings
(
    bool ApiKeyRequestHeaderRequired,
    string ApiKeyRequestHeaderName,
    string ApiKeyRequestHeaderValue,
    string Cors_AllowedOrigins,
    int FileUpload_MaxSizeInBytes
);