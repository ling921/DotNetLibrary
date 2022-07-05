using LingDev.Logging.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace LingDev.Logging;

/// <summary>
/// Creates <see cref="IChangeToken"/>s so that <see cref="IOptionsMonitor{TOptions}"/> gets notified when <see cref="IConfiguration"/> changes.
/// </summary>
/// <typeparam name="TFormatter">The implementation type of <see cref="FileFormatter"/>.</typeparam>
/// <typeparam name="TOptions">The implementation type of <see cref="FileFormatterOptions"/>.</typeparam>
public class FileLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions>
    : ConfigurationChangeTokenSource<TOptions>
    where TFormatter : FileFormatter
    where TOptions : FileFormatterOptions
{
    /// <summary>
    /// Constructor taking the <see cref="ILoggerProviderConfiguration{T}"/> instance to watch.
    /// </summary>
    /// <param name="providerConfiguration"></param>
    public FileLoggerFormatterOptionsChangeTokenSource(ILoggerProviderConfiguration<FileLoggerProvider> providerConfiguration)
        : base(providerConfiguration.Configuration.GetSection("FormatterOptions"))
    {
    }
}
