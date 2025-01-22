using Godot;

namespace Betauer.Tools.Logging;

public class GodotGdPrintWriter : ITextWriter {
    public static readonly GodotGdPrintWriter Instance = new();

    public void WriteLine(TraceLevel level, string line) => GD.Print(line);

    public void Flush() {
    }
}