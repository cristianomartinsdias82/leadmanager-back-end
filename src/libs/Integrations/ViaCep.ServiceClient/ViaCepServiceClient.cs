using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using ViaCep.ServiceClient.Configuration;
using ViaCep.ServiceClient.Models;

namespace ViaCep.ServiceClient
{
    internal sealed class ViaCepServiceClient : IViaCepServiceClient
    {
        private readonly HttpClient _http;
        private readonly ViaCepIntegrationSettings _settings;
        private readonly ILogger<ViaCepServiceClient> _logger;

        public ViaCepServiceClient(
            HttpClient http,
            ViaCepIntegrationSettings settings,
            ILogger<ViaCepServiceClient> logger)
        {
            _http = http;
            _settings = settings;
            _logger = logger;
        }

        public async Task<Endereco?> SearchAsync(string cep, CancellationToken cancellationToken)
        {
            try
            {
                var request = await _http.GetAsync(string.Format(_settings.Endpoint, Regex.Replace(cep, @"\D", string.Empty)), cancellationToken);

                return await request.Content.ReadFromJsonAsync<Endereco>(
                    cancellationToken: cancellationToken);
            }
            catch(Exception exc)
            {
                _logger?.LogError(exc, "{type} - {message}", exc.GetType().FullName, exc.Message);
            }

            return new Endereco() { Erro = true };
        }
    }
}