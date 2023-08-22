namespace CrossCutting.Messaging.RabbitMq.Configuration;

public sealed record RabbitMqConnectionSettings
(
    string HostName,
    int PortNumber,
    string UserName,
    string Password,
    string VirtualHost,
    string ClientProvidedName,
    int RequestedHeartbeatInSecs,
    bool AutomaticRecoveryEnabled
);