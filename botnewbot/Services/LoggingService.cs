using System;
using Discord;

namespace botnewbot.Services
{
    public class LoggingService
    {
        public static void Log(string message, LogSeverity severity = LogSeverity.Info)
        {
            DateTime dt = DateTime.Now;
            ConsoleColor color = setConsoleColorBySeverity(severity);
            Console.ForegroundColor = color;
            Console.WriteLine($"[{dt.ToString("HH:mm:ss")}] {message}");
        }
        private static ConsoleColor setConsoleColorBySeverity(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Debug:
                    return ConsoleColor.DarkGray;
                case LogSeverity.Info:
                    return ConsoleColor.White;
                case LogSeverity.Warning:
                    return ConsoleColor.Yellow;
                case LogSeverity.Error:
                    return ConsoleColor.Red;
                case LogSeverity.Critical:
                    return ConsoleColor.DarkRed;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}