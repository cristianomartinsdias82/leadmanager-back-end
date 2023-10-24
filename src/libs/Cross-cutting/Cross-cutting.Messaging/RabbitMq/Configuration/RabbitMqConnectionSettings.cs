namespace CrossCutting.Messaging.RabbitMq.Configuration;

public sealed record RabbitMqConnectionSettings
{
    public string HostName { get; init; } = default!;
    public int PortNumber { get; init; }
    public string UserName { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string VirtualHost { get; init; } = default!;
    public string ClientProvidedName { get; init; } = default!;
    public int RequestedHeartbeatInSecs { get; init; }
    public bool AutomaticRecoveryEnabled { get; init; }
    public int HealthCheckingTimeoutInSecs { get; init; }

    public string ConnectionString
        => $"amqp://{UserName}:{Password}@{HostName}:{PortNumber}/{((VirtualHost ?? string.Empty).Equals("/") ? string.Empty : VirtualHost)}";
}
