using Application.AddressSearch.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using ViaCep.ServiceClient.Configuration;
using ViaCep.ServiceClient.Models;
using ViaCep.ServiceClient.Mappings;

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

	public async Task<Address?> SearchByZipCodeAsync(string zipCode, CancellationToken cancellationToken = default)
	{
		try
		{
			var address = await _httpClient
							.GetFromJsonAsync<Endereco>(
											string.Format(_settings.Endpoint, Regex.Replace(zipCode, @"\D", string.Empty)),
											cancellationToken);

			return address.ToAddress();
		}
		catch (JsonException) { }
		catch (Exception exc)
		{
			_logger?.LogError(
				exc,
				"Error while attempting to communicate to ViaCep API | Type: {type} - Message: {message}",
				exc.GetType().FullName,
				exc.Message);
		}

		return default;
	}

	//public async Task<Endereco?> SearchAsync(string cep, CancellationToken cancellationToken)
	//{
	//	try
	//	{
	//		return await _httpClient
	//						.GetFromJsonAsync<Endereco?>(
	//							string.Format(_settings.Endpoint, Regex.Replace(cep, @"\D", string.Empty)),
	//							cancellationToken);
	//	}
	//	catch (JsonException) { }
	//	catch (Exception exc)
	//	{
	//		_logger?.LogError(
	//			exc, 
	//			"Error while attempting to communicate to ViaCep API | Type: {type} - Message: {message}",
	//			exc.GetType().FullName,
	//			exc.Message);
	//	}

	//	return new Endereco() { Erro = true };
	//}
}