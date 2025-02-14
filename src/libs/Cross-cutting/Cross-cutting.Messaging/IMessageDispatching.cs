namespace CrossCutting.Messaging;

public interface IMessageDispatching
{
    ValueTask SendToQueueAsync<T>(
        string queueName,
        T data,
		string eventType,
		CancellationToken cancellationToken = default);

    ValueTask SendToQueueAsync(
        string queueName,
        byte[] bytes,
		string eventType,
		CancellationToken cancellationToken = default);

    ValueTask SendToTopicAsync<T>(
        string topicName,
        string routingKey,
        T data,
		string eventType,
		CancellationToken cancellationToken = default);

    ValueTask SendToTopicAsync(
        string topicName,
        string routingKey,
        byte[] bytes,
		string eventType,
		CancellationToken cancellationToken = default);
}