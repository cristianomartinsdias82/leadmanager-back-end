namespace CrossCutting.Messaging;

public abstract class MessageDispatcher : IMessageDispatching
{
    public abstract ValueTask SendToQueueAsync<T>(
        string queueName,
        T data,
		string eventType,
		CancellationToken cancellationToken = default);

    public abstract ValueTask SendToQueueAsync(
        string queueName,
        byte[] bytes,
		string eventType,
		CancellationToken cancellationToken = default);
    
    public abstract ValueTask SendToTopicAsync<T>(
        string topicName,
        string routingKey,
        T data,
		string eventType,
        CancellationToken cancellationToken = default);

    public abstract ValueTask SendToTopicAsync(
        string topicName,
        string routingKey,
        byte[] bytes,
		string eventType,
		CancellationToken cancellationToken = default);
}
