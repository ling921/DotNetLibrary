using LingDev.Logging;
using LingDev.Logging.File;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for Dependency Injection.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Adds a file logger named 'File' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The <see cref="ILoggingBuilder"/>.</returns>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
    {
        builder.AddConfiguration();
        builder.AddFileFormatter<DefaultFileFormatter, FileFormatterOptions>();
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());
        LoggerProviderOptions.RegisterProviderOptions<FileLoggerOptions, FileLoggerProvider>(builder.Services);
        return builder;
    }

    /// <summary>
    /// Adds a file logger named 'File' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The <see cref="ILoggingBuilder"/>.</returns>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        builder.AddFile();
        builder.Services.Configure(configure);
        return builder;
    }

    /// <summary>
    /// Adds a custom console logger formatter 'TFormatter' to be configured with options 'TOptions'.
    /// </summary>
    /// <typeparam name="TFormatter">The implementation type of <see cref="FileFormatter"/>.</typeparam>
    /// <typeparam name="TOptions">The implementation type of <see cref="FileFormatterOptions"/>.</typeparam>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <returns>The <see cref="ILoggingBuilder"/>.</returns>
    public static ILoggingBuilder AddFileFormatter<TFormatter, TOptions>(this ILoggingBuilder builder)
        where TFormatter : FileFormatter
        where TOptions : FileFormatterOptions
    {
        builder.AddConfiguration();
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<FileFormatter, TFormatter>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<TOptions>, FileLoggerFormatterConfigureOptions<TFormatter, TOptions>>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<TOptions>, FileLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions>>());
        return builder;
    }
}
