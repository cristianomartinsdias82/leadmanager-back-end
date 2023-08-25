namespace CrossCutting.Messaging;

public abstract class MessageDispatcher : IMessageDispatching
{
    public abstract Task SendToQueueAsync<T>(string queueName, T data, CancellationToken cancellationToken = default);
    public abstract Task SendToQueueAsync(string queueName, byte[] bytes, CancellationToken cancellationToken = default);
    public abstract Task SendToTopicAsync<T>(string topicName, string routingKey, T data, CancellationToken cancellationToken = default);
    public abstract Task SendToTopicAsync(string topicName, string routingKey, byte[] bytes, CancellationToken cancellationToken = default);
}
