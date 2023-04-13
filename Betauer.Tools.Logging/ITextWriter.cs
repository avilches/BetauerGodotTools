namespace Betauer.Tools.Logging;

public interface ITextWriter {
    void WriteLine(TraceLevel level, string line);
    void Flush();
}