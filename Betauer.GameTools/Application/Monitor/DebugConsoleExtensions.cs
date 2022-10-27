using System;
using System.Linq;
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
        public static DebugConsole CreateCommand(this DebugConsole console, string name, Action<string[]> executeWithArguments, string shortHelp, string? longHelp = null) {
            return console.CreateCommand(name, (input) => executeWithArguments(input.Arguments), shortHelp, longHelp);
        }

        public static DebugConsole CreateCommand(this DebugConsole console, string name, Action execute, string shortHelp, string? longHelp = null) {
            return console.CreateCommand(name, executeWithCommandInput: (_) => execute(), shortHelp, longHelp);
        }

        public static DebugConsole CreateCommand(this DebugConsole console, string name,
            Action<DebugConsole.CommandInput> executeWithCommandInput, string shortHelp, string? longHelp = null) {
            return console.AddCommand(new DebugConsole.Command(name, executeWithCommandInput, shortHelp, longHelp));
        }

        public static DebugConsole AddHelpCommand(this DebugConsole console) {
            void OnHelp(string[] arguments) {
                if (arguments.Length == 0) {
                    var commands = console.Commands.Values.OrderBy(command => command.Name).ToList();
                    var maxLength = commands.Max(command => command.Name.Length);
                    console.WriteLine("Commands:");
                    console.WriteLine();
                    foreach (var command in commands) {
                        console.WriteLine($"    [color=#ffffff]{command.Name.PadRight(maxLength)}[/color] : {command.ShortHelp}");
                    }
                    console.WriteLine();
                    console.WriteLine("Type `help <command>` to get more info about a command.");
                    console.WriteLine("Press [color=#ffffff]Tab[/color] / [color=#ffffff]Shift+Tab[/color] to autocomplete commands.");
                    console.WriteLine("Press [color=#ffffff]Alt+Up[/color] / [color=#ffffff]Alt+Down[/color] to change the console size and position.");
                } else {
                    var commandName = arguments[0];
                    if (console.Commands.TryGetValue(commandName.ToLower(), out var command)) {
                        if (!string.IsNullOrWhiteSpace(command.LongHelp)) {
                            console.WriteLine(command.LongHelp);
                        } else if (!string.IsNullOrWhiteSpace(command.ShortHelp)) {
                            console.WriteLine(command.ShortHelp);
                        } else {
                            console.WriteLine($"No help for command `{commandName}`.");
                        }
                    } else {
                        console.WriteLine($"Error getting help: command `{commandName}` not found.");
                    }
                }
            }
            return console.CreateCommand("help", OnHelp, "Show this help.", @"Usage:
    [color=#ffffff]help          [/color] : List all available commands.
    [color=#ffffff]help <command>[/color] : Show help about a specific command.");
            
        }

        public static DebugConsole AddEngineTimeScaleCommand(this DebugConsole console) {
            const string help = @"Usage:
    [color=#ffffff]timescale        [/color] : Get the current time scale.
    [color=#ffffff]timescale <float>[/color] : Set the time scale to <number>.";
            return console.CreateCommand("timescale", (input) => {
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

        public static DebugConsole AddEngineTargetFpsCommand(this DebugConsole console) {
            const string help = @"Usage:
    [color=#ffffff]fps          [/color] : Get the current target fps.
    [color=#ffffff]fps <integer>[/color] : Set the target fps to <number>.";
            return console.CreateCommand("fps", (input) => {
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
            return console.CreateCommand("node-handler", () => {
                console.DebugOverlayManager
                    .Overlay("NodeHandler")
                    .Solid()
                    .Permanent(false)
                    .Enable()
                    .Text((nodeHandler ?? DefaultNodeHandler.Instance).GetStateAsString)
                    .UpdateEvery(1f);
            }, "Open the NodeHandler info window.");
        }

        public static DebugConsole AddSignalManagerCommand(this DebugConsole console, SignalManager? signalManager = null) {
            return console.CreateCommand("signals", () => {
                console.DebugOverlayManager
                    .Overlay("Signals")
                    .Permanent(false)
                    .Solid()
                    .Enable()
                    .Text((signalManager ?? DefaultSignalManager.Instance).GetStateAsString)
                    .UpdateEvery(1f);
            }, "Open the signals info window.");
        }

        public static DebugConsole AddClearConsoleCommand(this DebugConsole console) {
            return console.CreateCommand("clear", () => console.ClearConsole(), "Clear the console screen.");
        }

        public static DebugConsole AddShowAllCommand(this DebugConsole console) {
            return console.CreateCommand("show-all", () => console.DebugOverlayManager.ShowAllOverlays(), "Show all hidden windows.");
        }
    }
}