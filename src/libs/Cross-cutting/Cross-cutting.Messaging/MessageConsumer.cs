using System.Diagnostics;

namespace CrossCutting.Messaging;

public abstract class MessageConsumer : IMessageConsumption
{
	//public abstract void Subscribe(
	//    Func<byte[], bool> onMessageReceived,
	//    string queueName,
	//    string consumerIdentifier,
	//    IDictionary<string, object> arguments);

	public abstract void Subscribe<T>(
		Func<byte[], IEnumerable<ActivityLink>?, bool> onMessageReceived,
		string queueName,
		string consumerIdentifier,
		IDictionary<string, object> arguments);
}