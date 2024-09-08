using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
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

        const int MAX_ATTEMPTS = 5;
        var attemptCount = 1;

        do
        {
            logger!.LogInformation("Message bus configuration attempt #{attemptCount}...", attemptCount);

            try
            {
                using var channel = channelFactory.CreateChannel();

                foreach (var channelSettings in channelSettingsCollection)
                {
                    logger!.LogInformation("Declaring topic {TopicName}...", channelSettings.TopicName);
                    channel.ExchangeDeclare(
                        channelSettings.TopicName,
                        ExchangeType.Topic,
                        durable: true);
                    logger!.LogInformation("Topic {TopicName} declared successfully!", channelSettings.TopicName);

                    logger!.LogInformation("Declaring queue {QueueName}...", channelSettings.QueueName);

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
                    logger!.LogInformation("Queue {QueueName} declared successfully!", channelSettings.QueueName);


                    logger!.LogInformation("Declaring dead letter topic {DeadLetterExchange}...", channelSettings.DeadLetterExchange);
                    channel.ExchangeDeclare(
                        channelSettings.DeadLetterExchange,
                        ExchangeType.Direct,
                        durable: true);
                    logger!.LogInformation("Dead letter topic {DeadLetterExchange} declared successfully!", channelSettings.DeadLetterExchange);

                    logger!.LogInformation("Declaring dead letter queue {DeadLetterQueueName}...", channelSettings.DeadLetterQueueName);
                    channel.QueueDeclare(
                        queue: channelSettings.DeadLetterQueueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    logger!.LogInformation("Dead letter queue {DeadLetterQueueName} declared successfully!", channelSettings.DeadLetterQueueName);

                    logger!.LogInformation("Binding queue {QueueName} to topic {TopicName}...", channelSettings.QueueName, channelSettings.TopicName);
                    channel.QueueBind(
                        channelSettings.QueueName,
                        channelSettings.TopicName,
                        routingKey: channelSettings.RoutingKey);
                    logger!.LogInformation("Queue {QueueName} to topic {TopicName} bound successfully!", channelSettings.QueueName, channelSettings.TopicName);
                }

                logger!.LogInformation("Core messaging infrastructure initialized successfully.");
            }
            catch (Exception ex)
            {
                logger!.LogError(ex, "Core messaging infrastructure initialization error!");

                ++attemptCount;

                Thread.Sleep((int)Math.Pow(2, attemptCount) * 1000);

            }
        } while (attemptCount <= MAX_ATTEMPTS);

        if (attemptCount > MAX_ATTEMPTS)
            logger!.LogError("Unable to configure the message bus!");

        //Policy
        //.Handle<Exception>(ex =>
        //{
        //    logger?.LogError(ex, "Core messaging infrastructure initialization error!");

        //    return true;
        //})
        //.WaitAndRetry(5, count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)))
        //.Execute(() =>
        //{
        //    using var channel = channelFactory.CreateChannel();

        //    foreach (var channelSettings in channelSettingsCollection)
        //    {
        //        logger!.LogInformation("Declaring topic {TopicName}...", channelSettings.TopicName);
        //        channel.ExchangeDeclare(
        //            channelSettings.TopicName,
        //            ExchangeType.Topic,
        //            durable: true);
        //        logger!.LogInformation("Topic {TopicName} declared successfully!", channelSettings.TopicName);

        //        logger!.LogInformation("Declaring queue {QueueName}...", channelSettings.QueueName);

        //        channel.QueueDeclare(
        //            queue: channelSettings.QueueName,
        //            durable: channelSettings.Durable,
        //            exclusive: channelSettings.Exclusive,
        //            autoDelete: channelSettings.AutoDelete,
        //            arguments: new Dictionary<string, object>
        //            {
        //                    {"x-dead-letter-exchange", channelSettings.DeadLetterExchange},
        //                    {"x-dead-letter-routing-key", channelSettings.DeadLetterRoutingKey}
        //            });
        //        logger!.LogInformation("Queue {QueueName} declared successfully!", channelSettings.QueueName);


        //        logger!.LogInformation("Declaring dead letter topic {DeadLetterExchange}...", channelSettings.DeadLetterExchange);
        //        channel.ExchangeDeclare(
        //            channelSettings.DeadLetterExchange,
        //            ExchangeType.Direct,
        //            durable: true);
        //        logger!.LogInformation("Dead letter topic {DeadLetterExchange} declared successfully!", channelSettings.DeadLetterExchange);

        //        logger!.LogInformation("Declaring dead letter queue {DeadLetterQueueName}...", channelSettings.DeadLetterQueueName);
        //        channel.QueueDeclare(
        //            queue: channelSettings.DeadLetterQueueName,
        //            durable: true,
        //            exclusive: false,
        //            autoDelete: false,
        //            arguments: null);
        //        logger!.LogInformation("Dead letter queue {DeadLetterQueueName} declared successfully!", channelSettings.DeadLetterQueueName);

        //        logger!.LogInformation("Binding queue {QueueName} to topic {TopicName}...", channelSettings.QueueName, channelSettings.TopicName);
        //        channel.QueueBind(
        //            channelSettings.QueueName,
        //            channelSettings.TopicName,
        //            routingKey: channelSettings.RoutingKey);
        //        logger!.LogInformation("Queue {QueueName} to topic {TopicName} bound successfully!", channelSettings.QueueName, channelSettings.TopicName);
        //    }

        //    logger!.LogInformation("Core messaging infrastructure initialized successfully.");
        //});

        return app;
    }
}