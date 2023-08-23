namespace CrossCutting.Messaging;

public interface IMessageConsumption
{
    void Subscribe(
        Action<ReadOnlyMemory<byte>> messageReceived,
        IDictionary<string, object> arguments,
        string queueName);
}