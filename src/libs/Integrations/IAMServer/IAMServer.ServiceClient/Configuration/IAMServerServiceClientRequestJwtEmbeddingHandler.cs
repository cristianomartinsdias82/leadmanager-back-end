using Microsoft.AspNetCore.Http;

namespace IAMServer.ServiceClient.Configuration;

internal sealed class IAMServerServiceClientRequestJwtEmbeddingHandler : DelegatingHandler
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public IAMServerServiceClientRequestJwtEmbeddingHandler(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		const string AuthorizationHeaderKey = "Authorization";

		if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(AuthorizationHeaderKey, out var bearerToken))
			throw new InvalidOperationException("To make requests to IAM Server, a Json Web Token must be present.");

		request.Headers.Add(AuthorizationHeaderKey, [bearerToken]);

		return await base.SendAsync(request, cancellationToken);
	}
}