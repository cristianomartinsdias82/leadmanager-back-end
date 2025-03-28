using Microsoft.Extensions.Logging;

namespace ViaCep.ServiceClient.Configuration;

internal sealed class ViaCepServiceClientRequestAuditingHandler : DelegatingHandler
{
	private readonly ILogger<ViaCepServiceClientRequestAuditingHandler> _logger;

	public ViaCepServiceClientRequestAuditingHandler(ILogger<ViaCepServiceClientRequestAuditingHandler> logger)
	{
		_logger = logger;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		_logger.LogWarning("Sending request to {requestUri}", request.RequestUri?.AbsolutePath);

		return await base.SendAsync(request, cancellationToken);
	}
}
