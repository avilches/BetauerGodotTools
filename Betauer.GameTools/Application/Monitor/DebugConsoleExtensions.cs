using System;
using Godot;

namespace Betauer.Application.Monitor {
    public static class RichTextLabelExtensions {

        public static RichTextLabel AppendBbcodeLine(this RichTextLabel richTextLabel, string? text = null) {
            if (text != null) richTextLabel.AppendBbcode(text);
            richTextLabel.Newline();
            return richTextLabel;
        }

        public static RichTextLabel AddTextLine(this RichTextLabel richTextLabel, string? text = null) {
            if (text != null) richTextLabel.AddText(text);
            richTextLabel.Newline();
            return richTextLabel;
        }
    }
    
    public static class DebugConsoleExtensions {

        public static DebugConsole CreateCommand(this DebugConsole console, string name, Action<string[], RichTextLabel> execute, string shortHelp, string? longHelp = null) {
            return console.CreateCommand(name, (input, output) => execute(input.Arguments, output), shortHelp, longHelp);
        }
        
        public static DebugConsole CreateCommand(this DebugConsole console, string name, Action<string, RichTextLabel> execute, string shortHelp, string? longHelp = null) {
            return console.CreateCommand(name, (input, output) => execute(input.ArgumentString, output), shortHelp, longHelp);
        }
        
        public static DebugConsole CreateCommand(this DebugConsole console, string name, Action<RichTextLabel> execute, string shortHelp, string? longHelp = null) {
            return console.CreateCommand(name, (input, output) => {
                var _ = input.ArgumentString;
                execute(output);
            }, shortHelp, longHelp);
        }
        
        public static DebugConsole CreateCommand(this DebugConsole console, string name, Action<DebugConsole.CommandInput, RichTextLabel> execute, string shortHelp, string? longHelp = null) {
            return console.AddCommand(new DebugConsole.Command(name, execute, shortHelp, longHelp));
        }

        public static DebugConsole AddEngineTimeScale(this DebugConsole console) {
            const string help = @"Usage:
    [color=#ffffff]timescale        [/color] : Get the current time scale.
    [color=#ffffff]timescale <float>[/color] : Set the time scale to <number>.";
            return console.CreateCommand("timescale", (input, output) => {
                if (input.Arguments.Length == 0) {
                    output.AppendBbcodeLine($"Current time scale: {Engine.TimeScale.ToString()}");
                } else if (input.Arguments[0].IsValidFloat()) {
                    var newTimeScale = input.Arguments[0];
                    Engine.TimeScale = newTimeScale.ToFloat();
                    output.AppendBbcodeLine($"New time scale: {newTimeScale}");
                } else {
                    output.AppendBbcodeLine("Error: argument must be a valid integer.");
                    output.AppendBbcodeLine(help);
                }
            }, "Show or change the time scale.", help);
        }

        public static DebugConsole AddEngineTargetFps(this DebugConsole console) {
            const string help = @"Usage:
    [color=#ffffff]fps          [/color] : Get the current target fps.
    [color=#ffffff]fps <integer>[/color] : Set the target fps to <number>.";
            return console.CreateCommand("fps", (input, output) => {
                if (input.Arguments.Length == 0) {
                    output.AppendBbcodeLine($"Current target fps: {Engine.TargetFps.ToString()}");
                } else if (input.Arguments[0].IsValidInteger()) {
                    var newTargetFps = input.Arguments[0];
                    Engine.TargetFps = newTargetFps.ToInt();
                    output.AppendBbcodeLine($"New target fps: {newTargetFps}");
                } else {
                    output.AppendBbcodeLine("Error: argument must be a valid integer.");
                    output.AppendBbcodeLine(help);
                }
            }, "Show or change the target fps.", help);
        }

        public static DebugConsole AddQuit(this DebugConsole console) {
            const string help = @"Usage:
    [color=#ffffff]quit[/color] : Close the application safely.";
            return console.CreateCommand("quit", (input, output) => {
                var x = input.Arguments;
                output.AppendBbcodeLine("Quit game, please wait...");
                output.GetTree().Notification(MainLoop.NotificationWmQuitRequest);
            }, "Close the application safely.", help);
        }

        public static DebugConsole AddClearConsole(this DebugConsole console) {
            const string help = @"Usage:
    [color=#ffffff]clear[/color] : Clear the console screen.";
            return console.CreateCommand("clear", (output) => output.Text = "",
                "Clear the console screen.", help);
        }

        
    }
}