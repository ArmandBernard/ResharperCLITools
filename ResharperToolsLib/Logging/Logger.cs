using System;

namespace ResharperToolsLib.Logging
{
    public class Logger : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine(formatElements(message, LogLevel.Info));
        }

        public void Warn(string message)
        {
            throw new NotImplementedException();
        }

        public void Error(string message, Exception exception = null)
        {
            Console.Error.WriteLine(formatElements(message, LogLevel.Error, exception));
        }

        public void Fatal(string message, Exception exception = null)
        {
            Console.Error.WriteLine(formatElements(message, LogLevel.Fatal, exception));
        }

        private string formatElements(string message, LogLevel logLevel, Exception exception = null)
        {
            var output = $"[{DateTime.Now:U}]\t[{logLevel}]\t{message}";

            if (exception != null) output += $"\n{exception}";

            return output;
        }
    }
}