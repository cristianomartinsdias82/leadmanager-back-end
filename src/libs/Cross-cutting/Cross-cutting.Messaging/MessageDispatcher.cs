namespace CrossCutting.Messaging;

public abstract class MessageDispatcher : IMessageDispatching
{
    public abstract ValueTask SendToQueueAsync<T>(string queueName, T data, CancellationToken cancellationToken = default);
    public abstract ValueTask SendToQueueAsync(string queueName, byte[] bytes, CancellationToken cancellationToken = default);
    public abstract ValueTask SendToTopicAsync<T>(string topicName, string routingKey, T data, CancellationToken cancellationToken = default);
    public abstract ValueTask SendToTopicAsync(string topicName, string routingKey, byte[] bytes, CancellationToken cancellationToken = default);
}
