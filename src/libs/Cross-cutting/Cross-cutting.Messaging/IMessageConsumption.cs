namespace CrossCutting.Messaging;

public interface IMessageConsumption
{
    void Subscribe(
        Func<byte[], bool> onMessageReceived,
        string queueName,
        string consumerIdentifier,
        IDictionary<string, object> arguments);
}