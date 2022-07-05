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
    /// Gets or sets value indicating the minimum level of messages that would get written to error file. Defaults to <see cref="LogLevel.Error"/>.
    /// </summary>
    public LogLevel LogToErrorFile { get; set; } = LogLevel.Error;

    /// <summary>
    /// Gets or sets format string used to format timestamp in logging messages. Defaults to "G".
    /// </summary>
    public string? TimestampFormat { get; set; } = "G";

    /// <summary>
    /// Gets or sets indication whether or not UTC timezone should be used to for timestamps in logging messages. Defaults to false.
    /// </summary>
    public bool UseUtcTimestamp { get; set; }

    /// <summary>
    /// The file name of the log.
    /// <para>If log level great than <see cref="LogToErrorFile"/> and <see cref="ErrorFileName"/> is a valid filename, log would get written to error file.</para>
    /// <para>Default value is <c>Logs/${shortdate}.log</c>.</para>
    /// <para>Supported parameters: <c>shortdate</c>, <c>date</c></para>
    /// <para><c>${shortdate}</c> shortdate can not have format.</para>
    /// <para><c>${date:format}</c> date can customize date format.</para>
    /// </summary>
    public string LogFileName { get; set; } = "Logs/${shortdate}.log";

    /// <summary>
    /// The file name of the error log.
    /// <para>Default value is <c>Logs/${shortdate}.err.log</c>.</para>
    /// <para>Supported parameters: <c>shortdate</c>, <c>date</c></para>
    /// <para><c>${shortdate}</c> shortdate can not have format.</para>
    /// <para><c>${date:format}</c> date can customize date format.</para>
    /// </summary>
    public string ErrorFileName { get; set; } = "Logs/${shortdate}.err.log";
}
