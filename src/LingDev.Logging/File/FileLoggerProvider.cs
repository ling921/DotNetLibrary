using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace LingDev.Logging.File;

/// <summary>
/// A provider of <see cref="FileLogger"/> instances.
/// </summary>
[ProviderAlias("File")]
public class FileLoggerProvider : ILoggerProvider, IDisposable
{
    private readonly IOptionsMonitor<FileLoggerOptions> _options;

    private readonly ConcurrentDictionary<string, FileLogger> _loggers;

    private ConcurrentDictionary<string, FileFormatter> _formatters = new();

    private readonly FileLoggerProcessor _messageQueue;

    private readonly IDisposable? _optionsReloadToken;

    private readonly IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;

    /// <summary>
    /// Creates an instance of <see cref="FileLoggerProvider"/>.
    /// </summary>
    /// <param name="options">The options to create <see cref="FileLogger"/> instances with.</param>
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options)
        : this(options, Enumerable.Empty<FileFormatter>())
    {
    }

    /// <summary>
    /// Creates an instance of <see cref="FileLoggerProvider"/>.
    /// </summary>
    /// <param name="options">The options to create <see cref="FileLogger"/> instances with.</param>
    /// <param name="formatters">Log formatters added for <see cref="FileLogger"/> insteaces.</param>
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options, IEnumerable<FileFormatter>? formatters)
    {
        _options = options;
        _loggers = new ConcurrentDictionary<string, FileLogger>();
        SetFormatters(formatters);
        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = _options.OnChange(ReloadLoggerOptions);
        var writeTo = options.CurrentValue.WriteTo?.Length > 0
            ? options.CurrentValue.WriteTo
            : FileWriteConfiguration.Default;
        _messageQueue = new FileLoggerProcessor(options.CurrentValue.Path, writeTo);
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string name)
    {
        if (_options.CurrentValue.FormatterName == null || !_formatters.TryGetValue(_options.CurrentValue.FormatterName, out var value))
        {
            value = _formatters.ContainsKey("default")
                ? _formatters["default"]
                : new DefaultFileFormatter(new FormatterOptionsMonitor<FileFormatterOptions>(new FileFormatterOptions()));
            UpdateFormatterOptions(value, _options.CurrentValue);
        }
        if (!_loggers.TryGetValue(name, out var logger))
        {
            return _loggers.GetOrAdd(name, new FileLogger(name, _messageQueue, value, _scopeProvider, _options.CurrentValue));
        }
        return logger;
    }

    private static void UpdateFormatterOptions(FileFormatter formatter, FileLoggerOptions deprecatedFromOptions)
    {
        if (formatter is DefaultFileFormatter defaultFileFormatter)
        {
            defaultFileFormatter.FormatterOptions = new FileFormatterOptions
            {
                IncludeScopes = deprecatedFromOptions.IncludeScopes,
                TimestampFormat = deprecatedFromOptions.TimestampFormat,
                UseUtcTimestamp = deprecatedFromOptions.UseUtcTimestamp
            };
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
        GC.SuppressFinalize(this);
    }

    private void SetFormatters(IEnumerable<FileFormatter>? formatters = null)
    {
        var concurrentDictionary = new ConcurrentDictionary<string, FileFormatter>(StringComparer.OrdinalIgnoreCase);
        var flag = false;
        if (formatters != null)
        {
            foreach (FileFormatter formatter in formatters)
            {
                concurrentDictionary.TryAdd(formatter.Name, formatter);
                flag = true;
            }
        }
        if (!flag)
        {
            concurrentDictionary.TryAdd("default", new DefaultFileFormatter(new FormatterOptionsMonitor<FileFormatterOptions>(new FileFormatterOptions())));
        }
        _formatters = concurrentDictionary;
    }

    private void ReloadLoggerOptions(FileLoggerOptions options)
    {
        if (options.FormatterName == null || !_formatters.TryGetValue(options.FormatterName, out var value))
        {
            value = _formatters.ContainsKey("default")
                ? _formatters["default"]
                : new DefaultFileFormatter(new FormatterOptionsMonitor<FileFormatterOptions>(new FileFormatterOptions()));
            UpdateFormatterOptions(value, _options.CurrentValue);
        }
        foreach (KeyValuePair<string, FileLogger> logger in _loggers)
        {
            logger.Value.Options = options;
            logger.Value.Formatter = value;
        }
        var writeTo = options.WriteTo?.Length > 0
            ? options.WriteTo
            : FileWriteConfiguration.Default;
        _messageQueue?.Reload(options.Path, writeTo);
    }
}
