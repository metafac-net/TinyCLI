using Microsoft.Extensions.Logging;
using System;

namespace MiniCLI
{
    public class LogOptions
    {
        public LogLevel MinimumLevel { get; }
        public bool UseUtcTime { get; }
        public string TimeFormat { get; }
        public bool AllowMarkup { get; }
        public ILogger? ExternalLogger { get; }

        public LogOptions(
            LogLevel minimumLevel = LogLevel.Information,
            bool useUtcTime = false,
            string? timeFormat = null,
            bool allowMarkup = false,
            ILogger? externalLogger = null)
        {
            MinimumLevel = minimumLevel;
            UseUtcTime = useUtcTime;
            TimeFormat = timeFormat ?? "HH:mm:ss";
            AllowMarkup = allowMarkup;
            ExternalLogger = externalLogger;

            // early check for invalid formats
            string testTimeFormat = DateTime.Now.ToString(TimeFormat);
        }
    }
}