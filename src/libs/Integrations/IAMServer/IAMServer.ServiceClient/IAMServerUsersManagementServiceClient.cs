using Application.UsersManagement.UsersListing.Models;
using IAMServer.ServiceClient.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace IAMServer.ServiceClient;

internal sealed class IAMServerUsersManagementServiceClient : IIAMServerUsersManagementServiceClient
{
	private readonly HttpClient _httpClient;
	private readonly IAMServerIntegrationSettings _settings;
	private readonly ILogger<IAMServerUsersManagementServiceClient> _logger;

	public IAMServerUsersManagementServiceClient(
		HttpClient httpClient,
		IAMServerIntegrationSettings settings,
		ILogger<IAMServerUsersManagementServiceClient> logger)
	{
		_httpClient = httpClient;
		_settings = settings;
		_logger = logger;
	}

	public async Task<List<User>> ListUsersAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var users = await _httpClient
								.GetFromJsonAsync<List<User>>(
									_settings.Endpoint,
									cancellationToken);

			return users ?? [];
		}
		catch (JsonException jsonExc)
		{
			_logger?.LogError(
				jsonExc,
				"Error while attempting to communicate to IAM Server for getting the users list | Type: {type} - Message: {message}",
				jsonExc.GetType().FullName,
				jsonExc.Message);

			throw;
		}
		catch (Exception exc)
		{
			_logger?.LogError(
				exc,
				"Error while attempting to communicate to IAM Server for getting the users list | Type: {type} - Message: {message}",
				exc.GetType().FullName,
				exc.Message);

			throw;
		}
	}
}
