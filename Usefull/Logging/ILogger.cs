using System;

namespace ForgottenAdventuresTokenOrganizer.Usefull.Logging
{
    public interface ILogger
    {
        void Initialize(string logFileName);
        void Information(string line);
        void Warning(string line);
        void Error(Exception ex, string line);
        void CloseAndFlush();
    }
}
