using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LingDev.Logging.File;

/// <summary>
/// Allows custom log messages formatting
/// </summary>
public abstract class FileFormatter
{
    /// <summary>
    /// Gets the name associated with the console log formatter.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Creates a <see cref="FileFormatter"/>.
    /// </summary>
    /// <param name="name">The name of console log formatter.</param>
    /// <exception cref="ArgumentNullException">The name can not be null.</exception>
    protected FileFormatter(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        Name = name;
    }

    /// <summary>
    /// Writes the log message to the specified TextWriter.
    /// </summary>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="scopeProvider">The provider of scope data.</param>
    /// <param name="textWriter">The string writer.</param>
    public abstract void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter);
}
