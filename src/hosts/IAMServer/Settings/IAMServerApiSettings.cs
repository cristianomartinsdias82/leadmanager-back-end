namespace IAMServer.Settings;

public sealed record IAMServerApiSettings
{
    public string Authority { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public bool RequireHttpsMetadata { get; init; }
}