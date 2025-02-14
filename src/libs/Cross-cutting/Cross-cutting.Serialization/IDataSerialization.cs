namespace CrossCutting.Serialization;

public interface IDataSerialization
{
	byte[] Serialize<T>(T data);

	T Deserialize<T>(ReadOnlyMemory<byte> dataBytes);
}
