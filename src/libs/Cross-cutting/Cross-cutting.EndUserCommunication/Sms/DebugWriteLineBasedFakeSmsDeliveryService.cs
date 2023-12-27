
using System.Diagnostics;

namespace CrossCutting.EndUserCommunication.Sms;

internal sealed class DebugWriteLineBasedFakeSmsDeliveryService : ISmsDeliveryService
{
    public async Task SendAsync(string content, CancellationToken cancellationToken = default)
    {
        await Task.Delay(0, cancellationToken);

        Debug.WriteLine(content);
    }
}
