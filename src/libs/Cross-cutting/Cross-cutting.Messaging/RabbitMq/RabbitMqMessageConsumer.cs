using RabbitMQ.Client.Events;

namespace CrossCutting.Messaging.RabbitMq;

internal sealed class RabbitMqMessageConsumer : MessageConsumer
{
    private readonly IRabbitMqChannelFactory _rabbitMqChannelFactory;

    public RabbitMqMessageConsumer(IRabbitMqChannelFactory rabbitMqChannelFactory)
    {
        _rabbitMqChannelFactory = rabbitMqChannelFactory;
    }

    public override void Subscribe(
        Action<ReadOnlyMemory<byte>> messageReceived,
        IDictionary<string, object> arguments,
        string queueName)
    {
        using var channel = _rabbitMqChannelFactory.CreateChannel();
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            //TODO: Implement Polly Retry Pattern here!
            try
            {
                messageReceived(ea.Body);

                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch(Exception)
            {
                channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        channel.BasicConsume(queueName,
                             false,
                             string.Empty,
                             true,
                             false,
                             arguments,
                             consumer);
    }
}