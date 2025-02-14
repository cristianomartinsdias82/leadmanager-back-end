using CrossCutting.Messaging.RabbitMq.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;

namespace CrossCutting.Messaging.RabbitMq;

internal class RabbitMqChannelFactory : IRabbitMqChannelFactory
{
    private IConnection? _connection;
	private readonly ILogger<RabbitMqChannelFactory> _logger;

	public RabbitMqChannelFactory(
		ILogger<RabbitMqChannelFactory> logger,
		RabbitMqConnectionSettings rabbitMqConnectionSettings)
	{
		_logger = logger;

		Policy
			.Handle<Exception>(ex =>
			{
				_logger?.LogError(ex, "Core messaging infrastructure initialization error!");

				return true;
			})
			.WaitAndRetry(5, count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)))
			.Execute(() =>
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
			});
	}

	public IModel CreateChannel()
        => _connection!.CreateModel();
}
