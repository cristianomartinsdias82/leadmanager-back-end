using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CrossCutting.Messaging.RabbitMq.Configuration;

public static class RabbitMqHostExtensions
{
    public static IHost UseMessageBusInitialization(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<IHost>>();
        var channelFactory = scope.ServiceProvider.GetRequiredService<IRabbitMqChannelFactory>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var messageChannelSettings = configuration.GetSection(nameof(MessageChannelSettings)).Get<MessageChannelSettings>()!;
        var channelSettingsCollection = new List<ChannelSettings>
        {
            messageChannelSettings.NewlyRegisteredLeadsChannel,
            messageChannelSettings.UpdatedLeadChannel,
            messageChannelSettings.RemovedLeadChannel
        };

        /*
        //Resiliency logic
        return await Policy.Handle()
                .WaitAndRetryAsync(_settings.UploadAttemptsMaxCount, count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)))
                .ExecuteAsync(async () =>
        {
            
        });
        */

        //TODO: Wrap this code into a Policy.Execute block (from Polly library)
        //TODO: Log every step of the code below
        try
        {
            using var channel = channelFactory.CreateChannel();

            foreach (var channelSettings in channelSettingsCollection)
            {
                // Declare the exchange
                channel.ExchangeDeclare(
                    channelSettings.TopicName,
                    ExchangeType.Topic,
                    durable: true);

                // Declare the queue
                channel.QueueDeclare(
                    queue: channelSettings.QueueName,
                    durable: channelSettings.Durable,
                    exclusive: channelSettings.Exclusive,
                    autoDelete: channelSettings.AutoDelete,
                    arguments: new Dictionary<string, object>
                    {
                        {"x-dead-letter-exchange", channelSettings.DeadLetterExchange},
                        {"x-dead-letter-routing-key", channelSettings.DeadLetterRoutingKey}
                    });

                // Declare the dead-letter exchange
                channel.ExchangeDeclare(
                    channelSettings.DeadLetterExchange,
                    ExchangeType.Direct,
                    durable: true);

                // Declare the dead-letter queue
                channel.QueueDeclare(
                    queue: channelSettings.DeadLetterQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // Bind the main queue to the exchange
                channel.QueueBind(
                    channelSettings.QueueName,
                    channelSettings.TopicName,
                    routingKey: channelSettings.RoutingKey);
            }

            logger?.LogInformation("Core messaging infrastructure initialized successfully.");
        }
        catch (Exception exc)
        {
            logger?.LogError(exc, "Core messaging infrastructure initialization error!");
        }

        return app;
    }
}