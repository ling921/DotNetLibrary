using LingDev.Logging.File;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace LingDev.Logging;

/// <summary>
/// Configures an option for <see cref="FileLogger"/>.
/// </summary>
/// <typeparam name="TFormatter">The implementation type of <see cref="FileFormatter"/>.</typeparam>
/// <typeparam name="TOptions">The implementation type of <see cref="FileFormatterOptions"/>.</typeparam>
public class FileLoggerFormatterConfigureOptions<TFormatter, TOptions>
    : ConfigureFromConfigurationOptions<TOptions>
    where TFormatter : FileFormatter
    where TOptions : FileFormatterOptions
{
    /// <summary>
    /// Constructor that takes the <see cref="ILoggerProviderConfiguration{T}"/> instance to bind against.
    /// </summary>
    /// <param name="providerConfiguration">The configuration of provider.</param>
    public FileLoggerFormatterConfigureOptions(ILoggerProviderConfiguration<FileLoggerProvider> providerConfiguration)
        : base(providerConfiguration.Configuration.GetSection("FormatterOptions"))
    {
    }
}
