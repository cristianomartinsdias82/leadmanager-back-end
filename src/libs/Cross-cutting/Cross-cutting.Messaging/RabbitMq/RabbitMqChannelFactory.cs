using CrossCutting.Messaging.RabbitMq.Configuration;
using RabbitMQ.Client;

namespace CrossCutting.Messaging.RabbitMq;

internal class RabbitMqChannelFactory : IRabbitMqChannelFactory
{
    private readonly IConnection _connection;

    public RabbitMqChannelFactory(RabbitMqConnectionSettings rabbitMqConnectionSettings)
    {
        _connection = new ConnectionFactory
        {
            HostName = rabbitMqConnectionSettings.HostName,
            Port = rabbitMqConnectionSettings.PortNumber,
            UserName = rabbitMqConnectionSettings.UserName,
            Password = rabbitMqConnectionSettings.Password,
            VirtualHost = rabbitMqConnectionSettings.VirtualHost,
            ClientProvidedName = rabbitMqConnectionSettings.ClientProvidedName,
            RequestedHeartbeat = TimeSpan.FromSeconds(Math.Abs(rabbitMqConnectionSettings.RequestedHeartbeatInSecs)),
            AutomaticRecoveryEnabled = rabbitMqConnectionSettings.AutomaticRecoveryEnabled
        }
        .CreateConnection();
    }

    public IModel CreateChannel()
        => _connection.CreateModel();
}
