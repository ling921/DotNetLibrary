using LingDev.EntityFrameworkCore.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LingDev.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension methods for <see cref="ModelBuilder"/> and <see cref="PropertyBuilder"/>.
/// </summary>
public static class ModelBuilderExteiosns
{
    /// <summary>
    /// If the property is <see cref="Nullable"/>, convert the <see langword="default"/> value to <see langword="null"/>, otherwise do nothing.
    /// </summary>
    /// <typeparam name="TProperty">The type of property.</typeparam>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/>.</param>
    /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
    public static PropertyBuilder<TProperty> HasValueDefaultToNullConversion<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
    {
        var propertyType = typeof(TProperty);
        var underlyingType = Nullable.GetUnderlyingType(propertyType);
        if (underlyingType?.IsValueType != true)
        {
            return propertyBuilder;
        }

        var converterType = typeof(NullableDefaultToNullConverter<>).MakeGenericType(underlyingType);
        var converter = (ValueConverter)Activator.CreateInstance(converterType)!;
        propertyBuilder.HasConversion(converter);

        return propertyBuilder;
    }

    /// <summary>
    /// If the property is <see cref="Nullable"/>, convert the <see langword="default"/> value to <see langword="null"/>, otherwise do nothing.
    /// </summary>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/>.</param>
    /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
    public static PropertyBuilder HasValueDefaultToNullConversion(this PropertyBuilder propertyBuilder)
    {
        var propertyType = propertyBuilder.Metadata.ClrType;
        var underlyingType = Nullable.GetUnderlyingType(propertyType);
        if (underlyingType?.IsValueType != true)
        {
            return propertyBuilder;
        }

        var converterType = typeof(NullableDefaultToNullConverter<>).MakeGenericType(underlyingType);
        var converter = (ValueConverter)Activator.CreateInstance(converterType)!;
        propertyBuilder.HasConversion(converter);

        return propertyBuilder;
    }
}
