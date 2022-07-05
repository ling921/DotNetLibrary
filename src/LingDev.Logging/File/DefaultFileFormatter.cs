using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LingDev.Logging.File;

internal class DefaultFileFormatter : FileFormatter, IDisposable
{
    private static readonly string _messagePadding = new(' ', GetLogLevelString(LogLevel.Information).Length + ": ".Length);
    private static readonly string _newLineWithMessagePadding = Environment.NewLine + _messagePadding;

    private readonly IDisposable? _optionsReloadToken;

    internal FileFormatterOptions FormatterOptions { get; set; } = null!;

    public DefaultFileFormatter(IOptionsMonitor<FileFormatterOptions> options)
        : base("default")
    {
        if (options.CurrentValue == null)
            throw new ArgumentNullException(nameof(options));

        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    private void ReloadLoggerOptions(FileFormatterOptions options)
    {
        FormatterOptions = options;
    }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        string text = logEntry.Formatter!(logEntry.State, logEntry.Exception);
        if (logEntry.Exception != null || text != null)
        {
            LogLevel logLevel = logEntry.LogLevel;
            string logLevelString = GetLogLevelString(logLevel);
            string? text2 = null;
            var timestampFormat = FormatterOptions.TimestampFormat;
            if (timestampFormat != null)
            {
                text2 = GetCurrentDateTime().ToString(timestampFormat);
            }
            if (text2 != null)
            {
                textWriter.Write(text2);
                textWriter.Write(Environment.NewLine);
            }
            if (logLevelString != null)
            {
                textWriter.Write(logLevelString);
            }
            CreateDefaultLogMessage(textWriter, in logEntry, text, scopeProvider);
        }
    }

    private void CreateDefaultLogMessage<TState>(TextWriter textWriter, in LogEntry<TState> logEntry, string message, IExternalScopeProvider? scopeProvider)
    {
        int id = logEntry.EventId.Id;
        var exception = logEntry.Exception;
        textWriter.Write(": ");
        textWriter.Write(logEntry.Category);
        textWriter.Write('[');
        Span<char> destination = stackalloc char[10];
        if (id.TryFormat(destination, out var charsWritten))
        {
            textWriter.Write(destination[..charsWritten]);
        }
        else
        {
            textWriter.Write(id.ToString());
        }
        textWriter.Write(']');

        textWriter.Write(Environment.NewLine);

        WriteScopeInformation(textWriter, scopeProvider);
        WriteMessage(textWriter, message);
        if (exception != null)
        {
            WriteMessage(textWriter, exception.ToString());
        }
    }

    private static void WriteMessage(TextWriter textWriter, string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            textWriter.Write(_messagePadding);
            WriteReplacing(textWriter, Environment.NewLine, _newLineWithMessagePadding, message);
            textWriter.Write(Environment.NewLine);
        }
        static void WriteReplacing(TextWriter writer, string oldValue, string newValue, string message)
        {
            string value = message.Replace(oldValue, newValue);
            writer.Write(value);
        }
    }

    private DateTimeOffset GetCurrentDateTime()
    {
        return !FormatterOptions.UseUtcTimestamp
            ? DateTimeOffset.Now
            : DateTimeOffset.UtcNow;
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
        };
    }

    private void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider? scopeProvider)
    {
        if (!FormatterOptions.IncludeScopes || scopeProvider == null)
        {
            return;
        }
        scopeProvider.ForEachScope((scope, state) =>
        {
            state.Write(_messagePadding);
            state.Write("=> ");
            state.Write(scope);
        }, textWriter);

        textWriter.Write(Environment.NewLine);
    }
}
