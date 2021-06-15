using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Pannotation.StartApp
{
    public class FileLogger : ILogger
    {
        private string filePath;
        private object _lock = new object();

        public FileLogger(string path)
        {
            filePath = path;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                var text = formatter(state, exception);

                if (logLevel > LogLevel.Information)
                {
                    lock (_lock)
                    {
                        try
                        {
                            File.AppendAllText(filePath, DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") + ": " + text + Environment.NewLine);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
