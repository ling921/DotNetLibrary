using System.Text.Json;

namespace LingDev.Logging.File;

/// <summary>
/// Options for the built-in file log formatter.
/// </summary>
public class FileFormatterOptions
{
    /// <summary>
    /// Includes scopes when true.
    /// </summary>
    public bool IncludeScopes { get; set; }

    /// <summary>
    /// Gets or sets format string used to format timestamp in logging messages. Defaults to 'd'(short date).
    /// </summary>
    public string? TimestampFormat { get; set; } = "d";

    /// <summary>
    /// Gets or sets indication whether or not UTC timezone should be used to format timestamps in logging messages. Defaults to false.
    /// </summary>
    public bool UseUtcTimestamp { get; set; }
}
