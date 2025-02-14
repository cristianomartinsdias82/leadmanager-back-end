namespace CrossCutting.Messaging.RabbitMq;

internal enum RabbitMqDeliveryMode : byte
{
	NonPersistent = 1,
	Persistent = 2
}