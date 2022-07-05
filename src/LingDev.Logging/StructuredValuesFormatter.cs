using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace LingDev.Logging;

/// <summary>
/// A structured values formatter.
/// </summary>
internal sealed class StructuredValuesFormatter
{
    private const string _nullValue = "(null)";

    private static readonly char[] _formatDelimiters = new char[2] { ',', ':' };

    private static readonly ConcurrentDictionary<string, StructuredValuesFormatter> _formatters = new();

    private readonly string _format;

    private readonly List<string> _valueNames = new();

    /// <summary>
    /// The original format string.
    /// </summary>
    public string OriginalFormat { get; }

    /// <summary>
    /// A list of format value names in original format.
    /// </summary>
    public IReadOnlyList<string> ValueNames => _valueNames;

    /// <summary>
    /// Initialize a <see cref="StructuredValuesFormatter"/> with a format string.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <exception cref="ArgumentNullException">The format string can not be null.</exception>
    public StructuredValuesFormatter(string format)
    {
        OriginalFormat = format ?? throw new ArgumentNullException(nameof(format));
        var sb = new StringBuilder();
        int num = 0;
        int length = format.Length;
        while (num < length)
        {
            int num2 = FindBraceIndex(format, '{', num, length);
            if (num == 0 && num2 == length)
            {
                _format = format;
                return;
            }
            int num3 = FindBraceIndex(format, '}', num2, length);
            if (num3 == length)
            {
                sb.Append(format.AsSpan(num, length - num));
                num = length;
                continue;
            }
            int num4 = FindIndexOfAny(format, _formatDelimiters, num2, num3);
            sb.Append(format.AsSpan(num, num2 - num + 1));
            sb.Append(_valueNames.Count);
            _valueNames.Add(format.Substring(num2 + 1, num4 - num2 - 1));
            sb.Append(format.AsSpan(num4, num3 - num4 + 1));
            num = num3 + 1;
        }
        _format = sb.ToString();
    }

    /// <summary>
    /// Get an instance of <see cref="StructuredValuesFormatter"/>.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <returns>A <see cref="StructuredValuesFormatter"/> instance for the format string.</returns>
    public static StructuredValuesFormatter GetFormatter(string? format)
    {
        if (format == null)
            format = "[null]";

        if (!_formatters.TryGetValue(format, out var _formatter))
        {
            _formatter = new StructuredValuesFormatter(format);
            _formatters.TryAdd(format, _formatter);
        }

        return _formatter;
    }

    /// <summary>
    /// Replaces the format items in a string with the string representations of corresponding objects in a specified array.
    /// </summary>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
    public string Format(params object?[] args)
    {
        if (args == null || args.Length == 0)
        {
            return _format;
        }

        var array = new object[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            array[i] = FormatArgument(args[i]);
        }
        return string.Format(CultureInfo.InvariantCulture, _format, array);
    }

    private static int FindBraceIndex(string format, char brace, int startIndex, int endIndex)
    {
        int result = endIndex;
        int i = startIndex;
        int num = 0;
        for (; i < endIndex; i++)
        {
            if (num > 0 && format[i] != brace)
            {
                if (num % 2 != 0)
                {
                    break;
                }
                num = 0;
                result = endIndex;
            }
            else
            {
                if (format[i] != brace)
                {
                    continue;
                }
                if (brace == '}')
                {
                    if (num == 0)
                    {
                        result = i;
                    }
                }
                else
                {
                    result = i;
                }
                num++;
            }
        }
        return result;
    }

    private static int FindIndexOfAny(string format, char[] chars, int startIndex, int endIndex)
    {
        int num = format.IndexOfAny(chars, startIndex, endIndex - startIndex);
        return num != -1 ? num : endIndex;
    }

    private static object FormatArgument(object? value)
    {
        if (value == null)
        {
            return _nullValue;
        }
        var valueType = EscapeNullableType(value.GetType());
        if (valueType.IsPrimitive || value is string)
        {
            return value;
        }

        return JsonSerializer.Serialize(value);
    }

    private static Type EscapeNullableType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        return underlyingType ?? type;
    }
}
