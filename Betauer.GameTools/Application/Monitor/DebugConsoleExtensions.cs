using System;
using Betauer.Nodes;
using Betauer.Signal;
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
        public static DebugConsole CreateCommand(this DebugConsole console, string name,
            Action<string[], RichTextLabel> execute, string shortHelp, string? longHelp = null) {
            return console.CreateCommand(name, (input, output) => execute(input.Arguments, output), shortHelp,
                longHelp);
        }

        public static DebugConsole CreateCommand(this DebugConsole console, string name,
            Action<string, RichTextLabel> execute, string shortHelp, string? longHelp = null) {
            return console.CreateCommand(name, (input, output) => execute(input.ArgumentString, output), shortHelp,
                longHelp);
        }

        public static DebugConsole CreateCommand(this DebugConsole console, string name, Action<RichTextLabel> execute,
            string shortHelp, string? longHelp = null) {
            return console.CreateCommand(name, (input, output) => {
                var _ = input.ArgumentString;
                execute(output);
            }, shortHelp, longHelp);
        }

        public static DebugConsole CreateCommand(this DebugConsole console, string name,
            Action<DebugConsole.CommandInput, RichTextLabel> execute, string shortHelp, string? longHelp = null) {
            return console.AddCommand(new DebugConsole.Command(name, execute, shortHelp, longHelp));
        }

        public static DebugConsole CreateCommand(this DebugConsole console, string name,
            Action execute, string shortHelp, string? longHelp = null) {
            return console.AddCommand(new DebugConsole.Command(name, (_,_) => execute(), shortHelp, longHelp));
        }

        public static DebugConsole AddEngineTimeScale(this DebugConsole console) {
            const string help = @"Usage:
    [color=#ffffff]timescale        [/color] : Get the current time scale.
    [color=#ffffff]timescale <float>[/color] : Set the time scale to <number>.";
            return console.CreateCommand("timescale", (input, output) => {
                if (input.Arguments.Length == 0) {
                    console.WriteLine($"Current time scale: {Engine.TimeScale.ToString()}");
                } else if (input.Arguments[0].IsValidFloat()) {
                    var newTimeScale = input.Arguments[0];
                    Engine.TimeScale = newTimeScale.ToFloat();
                    console.WriteLine($"New time scale: {newTimeScale}");
                } else {
                    console.WriteLine("Error: argument must be a valid integer.");
                    console.WriteLine(help);
                }
            }, "Show or change the time scale.", help);
        }

        public static DebugConsole AddEngineTargetFps(this DebugConsole console) {
            const string help = @"Usage:
    [color=#ffffff]fps          [/color] : Get the current target fps.
    [color=#ffffff]fps <integer>[/color] : Set the target fps to <number>.";
            return console.CreateCommand("fps", (input, output) => {
                if (input.Arguments.Length == 0) {
                    console.WriteLine($"Current target fps: {Engine.TargetFps.ToString()}");
                } else if (input.Arguments[0].IsValidInteger()) {
                    var newTargetFps = input.Arguments[0];
                    Engine.TargetFps = newTargetFps.ToInt();
                    console.WriteLine($"New target fps: {newTargetFps}");
                } else {
                    console.WriteLine("Error: argument must be a valid integer.");
                    console.WriteLine(help);
                }
            }, "Show or change the target fps.", help);
        }

        public static DebugConsole AddQuitCommand(this DebugConsole console) {
            return console.CreateCommand("quit", () => {
                console.WriteLine("Quit game, please wait...");
                console.GetTree().Notification(MainLoop.NotificationWmQuitRequest);
            }, "Close the application safely.");
        }

        public static DebugConsole AddNodeHandlerInfoCommand(this DebugConsole console, NodeHandler? nodeHandler = null) {
            return console.CreateCommand("show-node-handler", () => {
                console.DebugOverlayManager
                    .Overlay("NodeHandler")
                    .Solid()
                    .Permanent(false)
                    .Enable()
                    .Text((nodeHandler ?? DefaultNodeHandler.Instance).GetStateAsString)
                    .UpdateEvery(1f);
            }, "Open a window overlay with the NodeHandler info.");
        }

        public static DebugConsole AddSignalManagerCommand(this DebugConsole console, SignalManager? signalManager = null) {
            return console.CreateCommand("show-signals", () => {
                console.DebugOverlayManager
                    .Overlay("Signals")
                    .Permanent(false)
                    .Solid()
                    .Enable()
                    .Text((signalManager ?? DefaultSignalManager.Instance).GetStateAsString)
                    .UpdateEvery(1f);
            }, "Open a window overlay showing all active signals.");
        }

        public static DebugConsole AddClearConsoleCommand(this DebugConsole console) {
            return console.CreateCommand("clear", (output) => output.Text = "", "Clear the console screen.");
        }
    }
}