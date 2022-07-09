using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LingDev.Logging.File;

internal class FileLogger : ILogger
{
    private readonly string _name;

    private readonly FileLoggerProcessor _queueProcessor;

    [ThreadStatic]
    private static StringWriter? t_stringWriter;

    internal FileFormatter Formatter { get; set; }

    internal IExternalScopeProvider? ScopeProvider { get; set; }

    internal FileLoggerOptions Options { get; set; }

    public FileLogger(string name, FileLoggerProcessor loggerProcessor, FileFormatter formatter, IExternalScopeProvider? scopeProvider, FileLoggerOptions options)
    {
        _name = name;
        _queueProcessor = loggerProcessor;
        Formatter = formatter;
        ScopeProvider = scopeProvider;
        Options = options;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return ScopeProvider == null
            ? NullScope.Instance
            : ScopeProvider.Push(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel) || string.Equals(_name, "Microsoft.Hosting.Lifetime"))
        {
            return;
        }
        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        if (t_stringWriter == null)
        {
            t_stringWriter = new StringWriter();
        }
        var logEntry = new LogEntry<TState>(logLevel, _name, eventId, state, exception, StructuredMessageFormatter);
        Formatter.Write(in logEntry, ScopeProvider, t_stringWriter);
        var stringBuilder = t_stringWriter.GetStringBuilder();
        if (stringBuilder.Length != 0)
        {
            string message = stringBuilder.ToString();
            stringBuilder.Clear();
            if (stringBuilder.Capacity > 1024)
            {
                stringBuilder.Capacity = 1024;
            }
            _queueProcessor.EnqueueMessage(new LogMessageEntry(message, logLevel));
        }
    }

    internal static string StructuredMessageFormatter<TState>(TState state, Exception? exception)
    {
        if (state == null)
        {
            return "(State is null)";
        }

        if (state is IReadOnlyList<KeyValuePair<string, object?>> values)
        {
            var message = values.Count > 0 ? values[^1] : default;
            if (message.Key != "{OriginalFormat}")
            {
                return "(Can't find the original format)";
            }
            if (message.Value is not string format)
            {
                return "(Original format is not a string)";
            }

            var augumentCount = values.Count - 1;
            if (augumentCount == 0)
            {
                return format;
            }

            var arguments = new object?[augumentCount];
            for (var i = 0; i < augumentCount; i++)
            {
                arguments[i] = values[i].Value;
            }

            var formatter = StructuredValuesFormatter.GetFormatter(format);
            return formatter.Format(arguments);
        }

        return $"(Unknown state type: {state.GetType()})";
    }
}
