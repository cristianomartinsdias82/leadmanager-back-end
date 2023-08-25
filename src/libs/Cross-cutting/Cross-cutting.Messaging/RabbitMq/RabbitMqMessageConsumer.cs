using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace CrossCutting.Messaging.RabbitMq;

internal sealed class RabbitMqMessageConsumer : MessageConsumer
{
    private readonly IRabbitMqChannelFactory _rabbitMqChannelFactory;
    private readonly ILogger<RabbitMqMessageConsumer> _logger;

    public RabbitMqMessageConsumer(
        IRabbitMqChannelFactory rabbitMqChannelFactory,
        ILogger<RabbitMqMessageConsumer> logger)
    {
        _rabbitMqChannelFactory = rabbitMqChannelFactory;
        _logger = logger;
    }

    public override void Subscribe(
        Func<byte[], bool> onMessageReceived,
        string queueName,
        string consumerIdentifier,
        IDictionary<string, object> arguments)
    {
        var channel = _rabbitMqChannelFactory.CreateChannel();
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var acknowledge = false;
            try
            {
                acknowledge = onMessageReceived(ea.Body.ToArray());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when receiving message from queue {queueName} for consumer {consumerIdentifier}", queueName, consumerIdentifier);
            }
            finally
            {
                if (acknowledge)
                    channel.BasicAck(ea.DeliveryTag, false);
                else
                    channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        channel.BasicConsume(queueName,
                             false,
                             consumerIdentifier,
                             true,
                             false,
                             arguments,
                             consumer);
    }
}