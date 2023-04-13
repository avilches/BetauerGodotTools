using System;

namespace Betauer.Tools.Logging;

public class ConsoleWriter : ITextWriter {
    public static readonly ConsoleWriter Instance = new();

    public void WriteLine(TraceLevel level, string line) {
        Console.WriteLine(line);
    }

    public void Flush() {
    }
}