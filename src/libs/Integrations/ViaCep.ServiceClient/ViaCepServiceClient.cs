using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;
using ViaCep.ServiceClient.Configuration;
using ViaCep.ServiceClient.Models;

namespace ViaCep.ServiceClient;

internal sealed class ViaCepServiceClient : IViaCepServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ViaCepIntegrationSettings _settings;
    private readonly ILogger<ViaCepServiceClient> _logger;

    public ViaCepServiceClient(
        HttpClient httpClient,
        ViaCepIntegrationSettings settings,
        ILogger<ViaCepServiceClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings;
        _logger = logger;
    }

    public async Task<Endereco?> SearchAsync(string cep, CancellationToken cancellationToken)
    {
        var responseContent = string.Empty;

        try
        {
			var response = await _httpClient.GetAsync(
								                string.Format(_settings.Endpoint, Regex.Replace(cep, @"\D", string.Empty)),
								                cancellationToken);

            responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<Endereco>(responseContent);
		}
		catch (JsonException jsonExc)
		{
			_logger?.LogError(jsonExc, "Error while parsing the ViaCep API response for zipcode {zipCode}. The API response content is: {responseContent}", cep, responseContent);
		}
		catch (Exception exc)
        {
            _logger?.LogError(exc, "Error while attempting to communicate to ViaCep API | Type: {type} - Message: {message}", exc.GetType().FullName, exc.Message);
        }

        return new Endereco() { Erro = true };
    }
}