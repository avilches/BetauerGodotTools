using System;

namespace Betauer.Tools.Logging;

public class ColoredConsoleWriter : ITextWriter {
    public static readonly ColoredConsoleWriter Instance = new();

    public void WriteLine(TraceLevel level, string line) {
        switch (level) {
            case TraceLevel.Fatal:
            case TraceLevel.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(line);
                Console.ResetColor();
                break;
            case TraceLevel.Warning:
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(line);
                Console.ResetColor();
                break;
            default:
                Console.WriteLine(line);
                break;
        }
    }

    public void Flush() {
    }
}