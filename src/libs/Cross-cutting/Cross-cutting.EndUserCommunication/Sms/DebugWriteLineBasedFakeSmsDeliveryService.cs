
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CrossCutting.EndUserCommunication.Sms;

internal sealed class DebugWriteLineBasedFakeSmsDeliveryService : ISmsDeliveryService
{
	private readonly ILogger<DebugWriteLineBasedFakeSmsDeliveryService> _logger;

	public DebugWriteLineBasedFakeSmsDeliveryService(ILogger<DebugWriteLineBasedFakeSmsDeliveryService> logger)
	{
		_logger = logger;
	}

	public async Task SendAsync(string content, CancellationToken cancellationToken = default)
    {
        await Task.Delay(0, cancellationToken);
		_logger.LogWarning("SMS => {content}", content);

        Debug.WriteLine(content);
    }
}
