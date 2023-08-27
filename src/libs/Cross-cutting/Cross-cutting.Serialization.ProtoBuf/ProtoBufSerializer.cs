using ProtoBuf;

namespace CrossCutting.Serialization.ProtoBuf;

public static class ProtoBufSerializer
{
    public static byte[] Serialize<T>(T instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        using var ms = new MemoryStream();
        Serializer.Serialize(ms, instance);
        return ms.ToArray();
    }

    public static T Deserialize<T>(ReadOnlyMemory<byte> span)
        => Serializer.Deserialize<T>(span);
}