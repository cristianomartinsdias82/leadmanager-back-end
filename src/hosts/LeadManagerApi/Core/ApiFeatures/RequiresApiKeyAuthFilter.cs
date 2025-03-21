using LeadManagerApi.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LeadManagerApi.Core.ApiFeatures;

public sealed class RequiresApiKeyAuthFilter : IAuthorizationFilter
{
	private readonly LeadManagerApiSettings _apiSettings;

	public RequiresApiKeyAuthFilter(LeadManagerApiSettings apiSettings)
	{
		_apiSettings = apiSettings;
	}

	public void OnAuthorization(AuthorizationFilterContext context)
	{
		if (!_apiSettings.ApiKeyRequestHeaderRequired)
			return;

		if (!context.HttpContext.Request.Headers.TryGetValue(_apiSettings.ApiKeyRequestHeaderName!, out var apiKey) ||
			!string.CompareOrdinal(apiKey, _apiSettings.ApiKeyRequestHeaderValue).Equals(0))
			context.Result = new UnauthorizedResult();
	}
}