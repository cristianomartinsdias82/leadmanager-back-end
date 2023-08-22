using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CrossCutting.Messaging.RabbitMq;

internal sealed class RabbitMqMessageDispatcher : MessageDispatcher
{
    private readonly IRabbitMqChannelFactory _rabbitMqChannelFactory;

    public RabbitMqMessageDispatcher(IRabbitMqChannelFactory rabbitMqChannelFactory)
    {
        _rabbitMqChannelFactory = rabbitMqChannelFactory;
    }

    public override Task SendToQueueAsync<T>(string queueName, T data, CancellationToken cancellationToken = default)
    {
        using var channel = _rabbitMqChannelFactory.CreateChannel();
        channel.BasicPublish(
            string.Empty,
            queueName,
            default!,
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data)));

        return Task.CompletedTask;
    }

    public override Task SendToQueueAsync(string queueName, byte[] bytes, CancellationToken cancellationToken = default)
    {
        using var channel = _rabbitMqChannelFactory.CreateChannel();
        channel.BasicPublish(string.Empty, queueName, default!, bytes);

        return Task.CompletedTask;
    }

    public override Task SendToTopicAsync<T>(string topicName, T data, CancellationToken cancellationToken = default)
    {
        using var channel = _rabbitMqChannelFactory.CreateChannel();
        channel.BasicPublish(topicName, string.Empty, default!, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data)));

        return Task.CompletedTask;
    }

    public override Task SendToTopicAsync(string topicName, byte[] bytes, CancellationToken cancellationToken = default)
    {
        using var channel = _rabbitMqChannelFactory.CreateChannel();
        channel.BasicPublish(topicName, string.Empty, default!, bytes);

        return Task.CompletedTask;
    }
}