using MassTransit;
using MassTransit.NewIdProviders;

namespace Shared.Generators;

public static class IdGenerator
{
    static IdGenerator()
    {
        NewId.SetProcessIdProvider(new CurrentProcessIdProvider());
    }

    public static Guid NextId()
        => NewId.NextSequentialGuid();
}