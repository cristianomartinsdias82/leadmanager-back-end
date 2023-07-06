namespace ViaCep.ServiceClient
{
    public record ViaCepIntegrationSettings
    {
        public int RequestTimeoutInSeconds { get; set; }
        public string Uri { get; set; } = default!;
        public string Endpoint { get; set; } = default!;
    }
}