using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LingDev.EntityFrameworkCore.Converters;

internal class NullableDefaultToNullConverter<T> : ValueConverter<T?, T?>
    where T : struct
{
    public NullableDefaultToNullConverter()
        : base(
            v => v == null || v.Equals(default(T)) ? null : v,
            v => v)
    {
    }
}
