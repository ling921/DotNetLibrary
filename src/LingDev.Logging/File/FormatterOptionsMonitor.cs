using Microsoft.Extensions.Options;

namespace LingDev.Logging.File;

internal sealed class FormatterOptionsMonitor<TOptions>
    : IOptionsMonitor<TOptions>
    where TOptions : FileFormatterOptions
{
    public TOptions CurrentValue { get; }

    public FormatterOptionsMonitor(TOptions options)
    {
        CurrentValue = options;
    }

    public TOptions Get(string? name)
    {
        return CurrentValue;
    }

    public IDisposable? OnChange(Action<TOptions, string> listener)
    {
        listener.Invoke(CurrentValue, string.Empty);
        return null;
    }
}
