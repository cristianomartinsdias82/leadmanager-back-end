namespace CrossCutting.EndUserCommunication.Sms;

public interface ISmsDeliveryService
{
    Task SendAsync(string content, CancellationToken cancellationToken = default);
}