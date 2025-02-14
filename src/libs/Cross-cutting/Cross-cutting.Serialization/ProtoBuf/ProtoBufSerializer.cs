using ProtoBuf;

namespace CrossCutting.Serialization.ProtoBuf;

public sealed class ProtoBufSerializer : IDataSerialization
{
	public byte[] Serialize<T>(T data)
	{
		using var ms = new MemoryStream();
		Serializer.Serialize(ms, data);

		return ms.ToArray();
	}

	public T Deserialize<T>(ReadOnlyMemory<byte> dataBytes)
		=> Serializer.Deserialize<T>(dataBytes);
}