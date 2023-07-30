namespace LeadManagerApi.ApiFeatures;

public sealed class LeadManagerApiSettings
{
    public bool ApiKeyRequestHeaderRequired { get; set; } = default!;
    public string ApiKeyRequestHeaderName { get; set; } = default!;
    public string ApiKeyRequestHeaderValue { get; set; } = default!;
    public string Cors_AllowedOrigins { get; set; } = default!;
    public int FileUpload_MaxSizeInBytes { get; set; } = default!;
}