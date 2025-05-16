using Microsoft.Extensions.Logging;

namespace IAMServer.ServiceClient.Configuration;

internal sealed class IAMServerServiceClientRequestAuditingHandler : DelegatingHandler
{
	private readonly ILogger<IAMServerServiceClientRequestAuditingHandler> _logger;

	public IAMServerServiceClientRequestAuditingHandler(ILogger<IAMServerServiceClientRequestAuditingHandler> logger)
	{
		_logger = logger;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		_logger.LogWarning("{infoSource} - Sending request to {requestUri}", GetType().Name, request.RequestUri?.AbsolutePath);

		return await base.SendAsync(request, cancellationToken);
	}
}
