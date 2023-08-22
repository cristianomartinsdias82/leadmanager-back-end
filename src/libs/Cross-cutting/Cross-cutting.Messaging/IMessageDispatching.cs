namespace CrossCutting.Messaging;

public interface IMessageDispatching
{
    Task SendToQueueAsync<T>(
        string queueName,
        T data,
        CancellationToken cancellationToken = default);

    Task SendToQueueAsync(
        string queueName,
        byte[] bytes,
        CancellationToken cancellationToken = default);

    Task SendToTopicAsync<T>(
        string topicName,
        T data,
        CancellationToken cancellationToken = default);

    Task SendToTopicAsync(
        string topicName,
        byte[] bytes,
        CancellationToken cancellationToken = default);
}