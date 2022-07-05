using System.Collections.Concurrent;
using System.Text;

namespace LingDev.Logging.File;

internal class FileLoggerProcessor : IDisposable
{
    private const int _maxQueuedMessages = 1024;

    private readonly BlockingCollection<LogMessageEntry> _messageQueue = new(1024);

    private readonly Thread _outputThread;

    private readonly StreamWriter _fileWriter;

    private readonly StreamWriter? _errorFileWriter;

    public FileLoggerProcessor(string filePath, string errorFilePath)
    {
        EnsureDirectoryExists(filePath);
        _fileWriter = new StreamWriter(filePath, true, Encoding.UTF8)
        {
            AutoFlush = true,
        };
        if (!string.IsNullOrEmpty(errorFilePath))
        {
            EnsureDirectoryExists(errorFilePath);
            _errorFileWriter = new StreamWriter(errorFilePath, true, Encoding.UTF8)
            {
                AutoFlush = true,
            };
        }
        else
        {
            _errorFileWriter = null;
        }

        _outputThread = new Thread(ProcessLogQueue)
        {
            IsBackground = true,
            Name = "File logger queue processing thread",
        };
        _outputThread.Start();
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
        if (!entry.LogAsError)
        {
            _fileWriter.WriteLine(entry.Message);
        }
        else
        {
            _errorFileWriter?.WriteLine(entry.Message);
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
            _fileWriter?.Dispose();
            _errorFileWriter?.Dispose();
        }
        catch (ThreadStateException)
        {
        }
    }

    private static void EnsureDirectoryExists(string filePath)
    {
        var dir = new FileInfo(filePath).Directory?.FullName;
        if (dir != null && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }
}
