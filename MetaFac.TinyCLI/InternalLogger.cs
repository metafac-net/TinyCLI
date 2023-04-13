using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace MetaFac.TinyCLI
{

    public sealed class InternalLogger : ILogger
    {
        private readonly object _scopeLock = new object();
        private LogScope? _topScope = null;

        private readonly LogOptions _options;

        public InternalLogger(LogOptions options)
        {
            _options = options;
        }

        private void CleanupScopes()
        {
            lock (_scopeLock)
            {
                while (_topScope is not null && _topScope.IsDisposed)
                {
                    _topScope = _topScope.Previous;
                }
            }
        }
        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            lock (_scopeLock)
            {
                var externalScope = _options.ExternalLogger?.BeginScope(state);
                string name = state.ToString() ?? typeof(TState).Name;
                var newScope = new LogScope(name, externalScope, _topScope, CleanupScopes);
                _topScope = newScope;
                return newScope;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _options.MinimumLevel
                || (_options.ExternalLogger is not null ? _options.ExternalLogger.IsEnabled(logLevel) : false);
        }

        private readonly List<LogEntry> _entries = new List<LogEntry>();
        public int EntryCount => _entries.Count;
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            _options.ExternalLogger?.Log(logLevel, eventId, state, exception, formatter);

            if (logLevel < _options.MinimumLevel) return;

            var scopePath = _topScope?.ScopePath;
            var message = formatter(state, exception);
            var timestamp = _options.UseUtcTime ? DateTime.UtcNow : DateTime.Now;
            _entries.Add(new LogEntry(timestamp, logLevel, scopePath, message));
        }

        private static IEnumerable<char> Trimmed(string? input)
        {
            // states:
            // 0 before 1st printable
            // 1 during printable
            Queue<char> queue = new Queue<char>();
            bool inBody = false;
            if (input is not null)
            {
                foreach (char ch in input)
                {
                    if (char.IsControl(ch)) continue;

                    if (inBody)
                    {
                        if (char.IsWhiteSpace(ch))
                        {
                            // save whitespace
                            queue.Enqueue(ch);
                        }
                        else
                        {
                            while (queue.Count > 0)
                            {
                                yield return queue.Dequeue();
                            }
                            yield return ch;
                        }
                    }
                    else
                    {
                        if (!char.IsWhiteSpace(ch))
                        {
                            inBody = true;
                            yield return ch;
                        }
                    }
                }
            }
        }

        private static void AppendCh(StringBuilder result, char ch, bool allowMarkup)
        {
            if (allowMarkup)
            {
                result.Append(ch);
            }
            else
            {
                if (ch == '[') result.Append("[[");
                else if (ch == ']') result.Append("]]");
                else result.Append(ch);
            }
        }

        /// <summary>
        /// Cleans up string:
        /// - strips CR LF
        /// - escapes markup
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string FormatMessage(string? scope, string input, bool allowMarkup)
        {
            StringBuilder result = new StringBuilder();
            int emitted = 0;
            foreach (char ch in Trimmed(scope))
            {
                emitted++;
                AppendCh(result, ch, allowMarkup);
            }
            if (emitted > 0)
            {
                result.Append(' ');
            }
            foreach (char ch in Trimmed(input))
            {
                emitted++;
                AppendCh(result, ch, allowMarkup);
            }
            if (emitted == 0)
            {
                result.Append(' ');
            }
            return result.ToString();
        }

        public Table GetEntriesAsTable()
        {
            var table = new Table();
            table.Border = TableBorder.None;
            table.AddColumn("Time");
            table.AddColumn("Level");
            table.AddColumn("Message");
            table.HideHeaders();
            foreach (var entry in _entries)
            {
                Color levelColor = entry.LogLevel switch
                {
                    LogLevel.Critical => Color.Magenta1,
                    LogLevel.Error => Color.Red,
                    LogLevel.Warning => Color.Yellow,
                    LogLevel.Information => Color.Aqua,
                    LogLevel.Debug => Color.Green,
                    LogLevel.Trace => Color.Grey,
                    _ => Color.Grey
                };
                string levelAbbr = entry.LogLevel switch
                {
                    LogLevel.Critical => "Crit.",
                    LogLevel.Warning => "Warn.",
                    LogLevel.Information => "Info.",
                    _ => entry.LogLevel.ToString()
                };
                string message = FormatMessage(entry.Scope, entry.Message, _options.AllowMarkup);
                table.AddRow(
                    entry.Timestamp.ToString(_options.TimeFormat),
                    $"[{levelColor}]{levelAbbr}[/]",
                    $"[{levelColor}]{message}[/]");
            }
            return table;
        }

    }
}