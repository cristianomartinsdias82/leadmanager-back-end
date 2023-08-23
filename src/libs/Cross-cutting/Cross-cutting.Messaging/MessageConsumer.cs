namespace CrossCutting.Messaging;

public abstract class MessageConsumer : IMessageConsumption
{
    public abstract void Subscribe(
        Action<ReadOnlyMemory<byte>> messageReceived,
        IDictionary<string, object> arguments,
        string queueName);
}