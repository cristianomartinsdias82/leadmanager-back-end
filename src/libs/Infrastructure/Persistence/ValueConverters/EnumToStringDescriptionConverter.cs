using Shared.FrameworkExtensions;

namespace Infrastructure.Persistence.ValueConverters;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class EnumToStringDescriptionConverter<TEnum> : ValueConverter<TEnum, string> where TEnum : struct, Enum
{
    public EnumToStringDescriptionConverter()
        : base(
            v => v.GetEnumDescription(),
            v => (TEnum)Enum.Parse(typeof(TEnum), v))
    {
    }
}