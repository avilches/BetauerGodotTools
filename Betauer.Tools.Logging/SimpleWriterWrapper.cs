using System;

namespace Betauer.Tools.Logging;

public class SimpleWriterWrapper : ITextWriter {
    private readonly Action<TraceLevel, string> _writer;

    public SimpleWriterWrapper(Action<TraceLevel, string> writer) {
        _writer = writer;
    }

    public void WriteLine(TraceLevel level, string line) {
        _writer?.Invoke(level, line);
    }

    public void Flush() {
    }
}