using CrossCutting.Serialization.ProtoBuf;
using RabbitMQ.Client;

namespace CrossCutting.Messaging.RabbitMq;

internal sealed class RabbitMqMessageDispatcher : MessageDispatcher
{
    private readonly IRabbitMqChannelFactory _rabbitMqChannelFactory;

    public RabbitMqMessageDispatcher(IRabbitMqChannelFactory rabbitMqChannelFactory)
    {
        _rabbitMqChannelFactory = rabbitMqChannelFactory;
    }

    public override ValueTask SendToQueueAsync<T>(
        string queueName,
        T data,
        CancellationToken cancellationToken = default)
    {
        using var channel = _rabbitMqChannelFactory.CreateChannel();
        channel.BasicPublish(
            string.Empty,
            queueName,
            default!,
            ProtoBufSerializer.Serialize(data!));

        return new();
    }

    public override ValueTask SendToQueueAsync(
        string queueName,
        byte[] bytes,
        CancellationToken cancellationToken = default)
    {
        using var channel = _rabbitMqChannelFactory.CreateChannel();
        channel.BasicPublish(string.Empty, queueName, default!, bytes);

        return new();
    }

    public override ValueTask SendToTopicAsync<T>(
        string topicName,
        string routingKey,
        T data,
        CancellationToken cancellationToken = default)
    {
        using var channel = _rabbitMqChannelFactory.CreateChannel();
        channel.BasicPublish(
            topicName,
            routingKey,
            default!,
            ProtoBufSerializer.Serialize(data!));

        return new();
    }

    public override ValueTask SendToTopicAsync(
        string topicName,
        string routingKey,
        byte[] bytes,
        CancellationToken cancellationToken = default)
    {
        using var channel = _rabbitMqChannelFactory.CreateChannel();
        channel.BasicPublish(topicName, routingKey, default!, bytes);

        return new();
    }
}
