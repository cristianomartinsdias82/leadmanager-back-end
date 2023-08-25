public sealed record MessageChannelSettings
{
    public ChannelSettings NewlyRegisteredLeadsChannel { get; init; } = default!;
    public ChannelSettings UpdatedLeadChannel { get; init; } = default!;
    public ChannelSettings RemovedLeadChannel { get; init; } = default!;
}

public sealed record ChannelSettings(
    string QueueName,
    string TopicName,
    string RoutingKey,
    string DeadLetterQueueName,
    string DeadLetterExchange,
    string DeadLetterRoutingKey,
    bool Durable,
    bool Exclusive,
    bool AutoDelete);