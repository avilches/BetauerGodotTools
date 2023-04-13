using Godot;

namespace Betauer.Tools.Logging;

public class GDPrintWriter : ITextWriter {
    public static readonly GDPrintWriter Instance = new();

    public void WriteLine(TraceLevel level, string line) {
        switch (level) {
            case TraceLevel.Fatal:
            case TraceLevel.Error:
                GD.PushError(line);
                break;
            case TraceLevel.Warning:
                GD.PushWarning(line);
                break;
            default:
                GD.Print(line);
                break;
        }
    }

    public void Flush() {
    }
}