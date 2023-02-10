using System;

using Microsoft.Extensions.Logging;

namespace MiniCLI
{
    internal readonly struct LogEntry
    {
        public readonly DateTime Timestamp;
        public readonly LogLevel LogLevel;
        public readonly string? Scope;
        public readonly string Message;

        public LogEntry(DateTime timestamp, LogLevel logLevel, string? scope, string message)
        {
            Timestamp = timestamp;
            LogLevel = logLevel;
            Scope = scope;
            Message = message;
        }
    }
}
