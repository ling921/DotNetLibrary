using Microsoft.Extensions.Logging;

namespace LingDev.Logging.File;

/// <summary>
/// Options for a <see cref="FileLogger"/>.
/// </summary>
public class FileLoggerOptions
{
    /// <summary>
    /// Name of the log message formatter to use. Defaults to "default".
    /// </summary>
    public string? FormatterName { get; set; }

    /// <summary>
    /// Includes scopes when true.
    /// </summary>
    public bool IncludeScopes { get; set; }

    /// <summary>
    /// Gets or sets format string used to format timestamp in logging messages. Defaults to "G".
    /// </summary>
    public string? TimestampFormat { get; set; } = "G";

    /// <summary>
    /// Gets or sets indication whether or not UTC timezone should be used to for timestamps in logging messages. Defaults to false.
    /// </summary>
    public bool UseUtcTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the path where log files are saved. Defaults to "Logs".
    /// </summary>
    public string Path { get; set; } = "Logs";

    /// <summary>
    /// Gets or sets the configurations for file writing. Defaults to <see cref="FileWriteConfiguration.Default"/>.
    /// </summary>
    public FileWriteConfiguration[] WriteTo { get; set; } = Array.Empty<FileWriteConfiguration>();
}

/// <summary>
/// The configuration for file writing.
/// </summary>
public class FileWriteConfiguration
{
    /// <summary>
    /// Gets or sets the file name of log messages that would get written to.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets value indicating the minimum level of messages that would get written to file. Defaults to <see cref="LogLevel.None"/>.
    /// </summary>
    public LogLevel MinLevel { get; set; } = LogLevel.None;

    /// <summary>
    /// Gets or sets value indicating the maximum level of messages that would get written to file. Defaults to <see cref="LogLevel.None"/>.
    /// </summary>
    public LogLevel MaxLevel { get; set; } = LogLevel.None;

    /// <summary>
    /// Default configurations.
    /// <para>It contains:</para>
    /// <para><c>${date:yyyyMMdd}.log</c> for level is <see cref="LogLevel.Information"/> or <see cref="LogLevel.Warning"/>.</para>
    /// <para><c>${date:yyyyMMdd}.err.log</c> for level is <see cref="LogLevel.Error"/> or <see cref="LogLevel.Critical"/>.</para>
    /// </summary>
    public readonly static FileWriteConfiguration[] Default = new[]
    {
        new FileWriteConfiguration
        {
            Name =  "${date:yyyyMMdd}.log",
            MinLevel = LogLevel.Information,
            MaxLevel = LogLevel.Warning,
        },
        new FileWriteConfiguration
        {
            Name =  "${date:yyyyMMdd}.err.log",
            MinLevel = LogLevel.Error,
            MaxLevel = LogLevel.Critical,
        },
    };

    /// <summary>
    /// All configurations.
    /// <para>It contains:</para>
    /// <para><c>${date:yyyyMMdd}.trace.log</c> for level is <see cref="LogLevel.Trace"/>.</para>
    /// <para><c>${date:yyyyMMdd}.debug.log</c> for level is <see cref="LogLevel.Debug"/>.</para>
    /// <para><c>${date:yyyyMMdd}.infomation.log</c> for level is <see cref="LogLevel.Information"/>.</para>
    /// <para><c>${date:yyyyMMdd}.warning.log</c> for level is <see cref="LogLevel.Warning"/>.</para>
    /// <para><c>${date:yyyyMMdd}.error.log</c> for level is <see cref="LogLevel.Error"/>.</para>
    /// <para><c>${date:yyyyMMdd}.critical.log</c> for level is <see cref="LogLevel.Critical"/>.</para>
    /// </summary>
    public readonly static FileWriteConfiguration[] All = new[]
    {
        new FileWriteConfiguration
        {
            Name =  "${date:yyyyMMdd}.trace.log",
            MinLevel = LogLevel.Trace,
            MaxLevel = LogLevel.Trace,
        },
        new FileWriteConfiguration
        {
            Name =  "${date:yyyyMMdd}.debug.log",
            MinLevel = LogLevel.Debug,
            MaxLevel = LogLevel.Debug,
        },
        new FileWriteConfiguration
        {
            Name =  "${date:yyyyMMdd}.infomation.log",
            MinLevel = LogLevel.Information,
            MaxLevel = LogLevel.Information,
        },
        new FileWriteConfiguration
        {
            Name =  "${date:yyyyMMdd}.warning.log",
            MinLevel = LogLevel.Warning,
            MaxLevel = LogLevel.Warning,
        },
        new FileWriteConfiguration
        {
            Name =  "${date:yyyyMMdd}.error.log",
            MinLevel = LogLevel.Error,
            MaxLevel = LogLevel.Error,
        },
        new FileWriteConfiguration
        {
            Name =  "${date:yyyyMMdd}.critical.log",
            MinLevel = LogLevel.Critical,
            MaxLevel = LogLevel.Critical,
        },
    };
}
