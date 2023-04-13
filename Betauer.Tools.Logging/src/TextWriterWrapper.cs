using System.IO;

namespace Betauer.Tools.Logging;

public class TextWriterWrapper : ITextWriter {
    private readonly TextWriter _writer;

    public TextWriterWrapper(TextWriter writer) => _writer = writer;

    public void WriteLine(TraceLevel level, string line) => _writer.WriteLine(line);

    public void Flush() => _writer.Flush();
}