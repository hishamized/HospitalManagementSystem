using HMS.Domain.Logger.Interfaces;
using NLog;


namespace HMS.Domain.Logger.Services
{
    public class LogService : ILog
    {
        private static readonly NLog.ILogger Logger = LogManager.GetLogger("FileAndDBLogger");

        public void Debug(string message)
        {
            Logger.Debug(message);
        }

        public void Error(Exception ex, string message)
        {
            Logger.Error(ex, message);
        }

        public void Information(string message)
        {
            Logger.Info(message); // Use Info for NLog
        }

        public void Warning(string message)
        {
            Logger.Warn(message); // Use Warn for NLog
        }
    }
}
