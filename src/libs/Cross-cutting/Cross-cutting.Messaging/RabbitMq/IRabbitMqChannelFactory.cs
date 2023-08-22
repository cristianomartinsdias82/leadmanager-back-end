using RabbitMQ.Client;

namespace CrossCutting.Messaging.RabbitMq;

internal interface IRabbitMqChannelFactory
{
    IModel CreateChannel();
}
