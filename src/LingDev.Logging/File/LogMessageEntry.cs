using Microsoft.Extensions.Logging;

namespace LingDev.Logging.File;

internal readonly struct LogMessageEntry
{
    public readonly LogLevel LogLevel;

    public readonly string Message;

    public LogMessageEntry(string message, LogLevel logLevel)
    {
        LogLevel = logLevel;
        Message = message;
    }
}
