using System;

namespace ResharperToolsLib.Logging
{
    public enum LogLevel
    {
        Info,
        Warn,
        Error,
        Fatal
    }

    public interface ILogger
    {
        public void Info(string message);

        public void Warn(string message);

        public void Error(string message, Exception exception = null);

        public void Fatal(string message, Exception exception = null);
    }
}