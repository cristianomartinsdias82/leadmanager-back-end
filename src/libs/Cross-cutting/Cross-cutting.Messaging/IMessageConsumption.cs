using System.Diagnostics;

namespace CrossCutting.Messaging;

public interface IMessageConsumption
{
	//void Subscribe(
	//    Func<byte[], bool> onMessageReceived,
	//    string queueName,
	//    string consumerIdentifier,
	//    IDictionary<string, object> arguments);

	void Subscribe<T>(
		Func<byte[], IEnumerable<ActivityLink>?, bool> onMessageReceived,
		string queueName,
		string consumerIdentifier,
		IDictionary<string, object> arguments);
}