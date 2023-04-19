using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ForgottenAdventuresTokenOrganizer.Usefull.Logging
{
    public class Logger : ILogger
    {
        private static readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private static bool _initialized;
        private static string _logFilePath = string.Empty;

        public Logger()
        {
            Initialize("log.txt");
        }

        public Logger(string logFileName)
        {
            Initialize(logFileName);
        }

        public void Initialize(string logFileName)
        {
            if (!_initialized)
            {
                _initialized = true;
                _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFileName);
                if(File.Exists(_logFilePath)) { File.Delete(_logFilePath); }
                Task.Run(() =>
                {
                    while (_initialized)
                    {
                        AppendAllLines();
                        Thread.Sleep(500);
                    }
                });
                Information("Logger successfully initialized!");
            }
            else
            {
                Information("Logger already initialized!");
            }
        }

        public void Information(string line)
        {
            LogLine($"[INFO] : {line}");
        }

        public void Warning(string line)
        {
            LogLine($"[WARN] : {line}");
        }

        public void Error(Exception ex, string line)
        {
            LogLine($"[ERROR] : {line} StackTrace : {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Error(ex.InnerException, ex.Message);
            }
        }

        public void CloseAndFlush()
        {
            if (_initialized)
            {
                Information("Closing and flushing Logger!");
                _initialized = false;
                AppendAllLines();
            }
        }

        private static void LogLine(string line)
        {
            if (_initialized)
            {
                _logQueue.Enqueue($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff tt} [{Environment.CurrentManagedThreadId}] {line}");
            }
        }

        private static void AppendAllLines()
        {
            lock (_logFilePath)
            {
                var lines = new List<string>();
                while (_logQueue.TryDequeue(out var line))
                {
                    lines.Add(line);
                }
                File.AppendAllLines(_logFilePath, lines);
            }
        }
    }
}
