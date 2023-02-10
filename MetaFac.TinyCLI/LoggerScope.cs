using System;

namespace MetaFac.TinyCLI
{
    internal sealed class LogScope : IDisposable
    {
        private readonly string _scopePath;
        private readonly IDisposable? _externalScope;
        private readonly LogScope? _previous;
        private readonly Action _cleanup;

        private volatile bool _disposed = false;

        public string ScopePath => _scopePath;
        public LogScope? Previous => _previous;
        public bool IsDisposed => _disposed;

        public LogScope(string name, IDisposable? externalScope, LogScope? previous, Action cleanup)
        {
            _scopePath = previous is null ? name : $"{previous.ScopePath}/{name}";
            _externalScope = externalScope;
            _previous = previous;
            _cleanup = cleanup;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _externalScope?.Dispose();
            _cleanup();
        }
    }
}
