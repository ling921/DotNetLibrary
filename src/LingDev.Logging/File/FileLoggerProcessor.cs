using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace LingDev.Logging.File;

internal class FileLoggerProcessor : IDisposable
{
    private const int _maxQueuedMessages = 1024;

    private readonly BlockingCollection<LogMessageEntry> _messageQueue = new(1024);

    private readonly Thread _outputThread;

    private readonly ICollection<FileWriter> _writers = new List<FileWriter>();

    public FileLoggerProcessor(string path, IEnumerable<FileWriteConfiguration> writeConfigurations)
    {
        _outputThread = new Thread(ProcessLogQueue)
        {
            IsBackground = true,
            Name = "File logger queue processing thread",
        };

        Reload(path, writeConfigurations);

        _outputThread.Start();
    }

    internal void Reload(string path, IEnumerable<FileWriteConfiguration> writeConfigurations)
    {
        EnsureDirectoryExists(path);

        foreach (var writer in _writers)
        {
            writer.Dispose();
        }

        _writers.Clear();
        var validConfigurations = writeConfigurations
            .Where(c => c.MinLevel < LogLevel.None && c.MinLevel <= c.MaxLevel)
            .DistinctBy(c => c.Name);
        foreach (var configuration in validConfigurations)
        {
            var fileName = Path.Combine(path, configuration.Name);
            _writers.Add(new FileWriter(fileName, configuration.MinLevel, configuration.MaxLevel));
        }
    }

    public virtual void EnqueueMessage(LogMessageEntry message)
    {
        if (!_messageQueue.IsAddingCompleted)
        {
            try
            {
                _messageQueue.Add(message);
                return;
            }
            catch (InvalidOperationException)
            {
            }
        }
        try
        {
            WriteMessage(message);
        }
        catch
        {
        }
    }

    internal void WriteMessage(LogMessageEntry entry)
    {
        foreach (var writer in _writers)
        {
            writer.WriteMessage(entry);
        }
    }

    private void ProcessLogQueue()
    {
        try
        {
            foreach (var item in _messageQueue.GetConsumingEnumerable())
            {
                WriteMessage(item);
            }
        }
        catch
        {
            try
            {
                _messageQueue.CompleteAdding();
            }
            catch
            {
            }
        }
    }

    public void Dispose()
    {
        _messageQueue.CompleteAdding();
        try
        {
            _outputThread.Join(1500);
            foreach (var writer in _writers)
            {
                writer.Dispose();
            }
        }
        catch (ThreadStateException)
        {
        }
    }

    private static void EnsureDirectoryExists(string filePath)
    {
        var dir = new DirectoryInfo(filePath).FullName;
        if (dir != null && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    private class FileWriter : IDisposable
    {
        private readonly StreamWriter _writer;
        private readonly LogLevel _min;
        private readonly LogLevel _max;

        public FileWriter(string fileNameFormat, LogLevel min, LogLevel max)
        {
            var fileName = FormatFileName(fileNameFormat, DateTime.Now);
            _writer = new StreamWriter(fileName, true, Encoding.UTF8)
            {
                AutoFlush = true,
            };
            _min = min;
            _max = max;
        }

        internal void WriteMessage(LogMessageEntry entry)
        {
            if (entry.LogLevel != LogLevel.None && _min <= entry.LogLevel && _max >= entry.LogLevel)
            {
                _writer.WriteLine(entry.Message);
            }
        }

        public void Dispose()
        {
            _writer?.Flush();
            _writer?.Close();
            _writer?.Dispose();
        }

        private static string FormatFileName(string format, DateTime date)
        {
            format = Regex.Replace(format, @"\${date(:.+)?}", "{0$1}");
            return string.Format(format, date);
        }
    }
}
