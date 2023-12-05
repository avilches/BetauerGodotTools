using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Betauer.Application.Screen;
using Betauer.Input;
using Betauer.Nodes;
using Godot;

namespace Betauer.Application.Monitor; 

public static class RichTextLabelExtensions {
    public static RichTextLabel AppendBbcodeLine(this RichTextLabel richTextLabel, string? text = null) {
        if (text != null) richTextLabel.AppendText(text);
        richTextLabel.Newline();
        return richTextLabel;
    }

    public static RichTextLabel AddTextLine(this RichTextLabel richTextLabel, string? text = null) {
        if (text != null) richTextLabel.AddText(text);
        richTextLabel.Newline();
        return richTextLabel;
    }
}

public static partial class DebugConsoleExtensions {
    public static DebugConsole.ConditionalCommand CreateCommand(this DebugConsole console, string name, string shortHelp, string? longHelp = null, string? errorMessage = null) {
        var command = new DebugConsole.ConditionalCommand(console, name, shortHelp, longHelp, errorMessage);
        console.AddCommand(command);
        return command;
    }

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

    public static DebugConsole AddAllCommands(this DebugConsole console) {
        console.AddHelpCommand();
        console.AddEngineTimeScaleCommand();
        console.AddEngineMaxFpsCommand();
        console.AddClearConsoleCommand();
        console.AddQuitCommand();
        console.AddShowAllCommand();
        console.AddSystemInfoCommand();
        return console;
    }

    public static DebugConsole AddHelpCommand(this DebugConsole console) {
        void ShowHelp() {
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
        }
        void HelpOnCommand(string commandName) {
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
        return console.CreateCommand("help", "Show this help.", @"Usage:
    [color=#ffffff]help          [/color] : List all available commands.
    [color=#ffffff]help <command>[/color] : Show help about a specific command.")
            .WithNoArguments(ShowHelp)
            .ArgumentIsPresent(input => HelpOnCommand(input.Arguments[0]))
            .End();
            
    }

    public static DebugConsole AddEngineTimeScaleCommand(this DebugConsole console) {
        return console.CreateCommand("timescale",
                "Show or change the time scale.",
                @"Usage:
    [color=#ffffff]timescale        [/color] : Get the current time scale.
    [color=#ffffff]timescale <float>[/color] : Set the time scale to <float>.",
                "Error: argument must be a valid float number.")
            .WithNoArguments(() => {
                console.WriteLine($"Current time scale: {Engine.TimeScale.ToString(CultureInfo.InvariantCulture)}");
            })
            .ArgumentIsFloat(input => {
                var newTimeScale = input.Arguments[0];
                Engine.TimeScale = double.Parse(newTimeScale, NumberStyles.Float, CultureInfo.InvariantCulture);
                console.WriteLine($"New time scale: {newTimeScale}");
            }).End();
    }
                                                                        
    public static DebugConsole AddEngineMaxFpsCommand(this DebugConsole console) {
        return console.CreateCommand("fps",
                "Show or change the target fps.",
                @"Usage:
    [color=#ffffff]fps      [/color] : Get the current target fps.
    [color=#ffffff]fps <int>[/color] : Set the target fps to <int>.",
                "Error: argument must be a valid integer.")
            .WithNoArguments(() => {
                console.WriteLine($"Current target fps: {Engine.MaxFps.ToString()}");
            })
            .ArgumentIsInteger(input => {
                var newMaxFps = input.Arguments[0];
                Engine.MaxFps = newMaxFps.ToInt();
                console.WriteLine($"New target fps: {newMaxFps}");
            }).End();
    }

    public static DebugConsole AddQuitCommand(this DebugConsole console) {
        return console.CreateCommand("quit", 
                "Quit the application safely with exit code",
                @"Usage:
    [color=#ffffff]quit      [/color] : Quit the application with exit code 0.
    [color=#ffffff]quit <int>[/color] : Quit the application with exit code <int>.",
                "Error: argument must be a valid integer.")
            .WithNoArguments(() => {
                console.WriteLine("Quit game with exit code 0. Please wait...");
                console.GetTree().QuitSafely(0);
            })
            .ArgumentIsInteger(input => {
                var exitCode = input.Arguments[0].ToInt();
                console.WriteLine($"Quit game with exit code {exitCode.ToString()}. Please wait...");
                console.GetTree().QuitSafely(exitCode);
            }).End();
    }

    public static DebugConsole AddSystemInfoCommand(this DebugConsole console) {
        const string title = "System info";
        return console.CreateCommand("system-info", () => {
            if (console.DebugOverlayManager.HasOverlay(title)) return;
            console.DebugOverlayManager
                .Overlay(title)
                .HideOnClose(false)
                .Solid()
                .Children()
                .AddWindowNotificationStatus()
                .AddMonitorFpsTimeScaleAndUptime()
                .AddMonitorMemory()
                .AddMonitorInternals();
        }, "Open the system info window.");
    }

    public static DebugConsole AddScreenSettingsCommand(this DebugConsole console, ScreenSettingsManager screenSettingsManager) {
        const string title = nameof(ScreenSettingsManager);
        return console.CreateCommand("screen-settings", () => {
            if (console.DebugOverlayManager.HasOverlay(title)) return;
            console.DebugOverlayManager
                .Overlay(title)
                .HideOnClose(false)
                .Solid()
                .Children()
                .AddMonitorVideoInfo()
                .AddMonitorScreenSettings(screenSettingsManager);
        }, "Open the screen settings info window.");
    }

    public static DebugConsole AddInputMapCommand(this DebugConsole console, InputActionsContainer inputActionsContainer) {
        const string title = nameof(InputActionsContainer);
        return console.CreateCommand("input-map", () => {
            if (console.DebugOverlayManager.HasOverlay(title)) return;
            console.DebugOverlayManager
                .Overlay(title)
                .HideOnClose(false)
                .Solid()
                .Children()
                .AddMonitorInputAction(inputActionsContainer);
        }, "Open the input map window.");
    }

    public static DebugConsole AddInputEventCommand(this DebugConsole console, InputActionsContainer inputActionsContainer, int history = 10) {
        const string title = "Input event logger";
        return console.CreateCommand("input-logger", () => {
            if (console.DebugOverlayManager.HasOverlay(title)) return;

            var inputs = new LinkedList<string>();
            var nodeHandler = NodeManager.MainInstance.OnInput((e) => {
                var pressed = e.IsPressed() ? "Pressed" + (e.IsJustPressed() ? " (Just)" : "") :
                    e.IsReleased() ? "Released" : "Unknown";
                var modifiers = new List<string>(5);
                if (e.HasShift()) modifiers.Add("Shift");
                if (e.HasAlt()) modifiers.Add("Alt");
                if (e.HasControl()) modifiers.Add("Ctrl");
                if (e.HasMeta()) modifiers.Add("Meta");
                var actions = inputActionsContainer.InputActionList.Where(a => a.IsEvent(e)).Select(a => a.Name).ToList();
                var actionName = actions.Count > 0 ? $" | Action [{string.Join(",", actions)}]" : "";
                if (e.IsAnyKey()) {
                    modifiers.Add(e.GetKeyString());
                    inputs.AddLast($"Key {(int)e.GetKey()} [{string.Join('+', modifiers)}] {pressed} {actionName}");
                } else if (e.IsAnyClick()) {
                    modifiers.Add(e.GetClick().ToString());
                    inputs.AddLast($"Click {(int)e.GetClick()} [{string.Join('+', modifiers)}] {pressed} {actionName}");
                } else if (e.IsAnyButton()) {
                    modifiers.Add(e.GetButton().ToString());
                    inputs.AddLast($"Button {(int)e.GetButton()} [{string.Join('+', modifiers)}] {pressed} | {e.GetButtonPressure()} {actionName}");
                } else if (e.IsAnyAxis())
                    inputs.AddLast($"Axis {(int)e.GetAxis()} [{e.GetAxis()}] {e.GetAxisValue():0.00} {actionName}");
                // else if (e.IsMouseMotion())
                // inputs.AddLast($"Mouse motion {e.GetMouseGlobalPosition()} {actionName}");
                if (inputs.Count > history) inputs.RemoveFirst();
            });

            console.DebugOverlayManager
                .Overlay(title)
                .HideOnClose(false)
                .OnVisible(visible => nodeHandler.Enable(visible))
                .OnDestroy(() => nodeHandler.Destroy())
                .Solid()
                .SetMinSize(400, 200)
                .Children()
                .TextField("", () => string.Join('\n', inputs));
            
        }, "Open the input event logger window.");
    }

    public static DebugConsole AddClearConsoleCommand(this DebugConsole console) {
        return console.CreateCommand("clear", () => console.ClearConsole(), "Clear the console screen.");
    }

    public static DebugConsole AddShowAllCommand(this DebugConsole console) {
        return console.CreateCommand("show-all", () => console.DebugOverlayManager.ShowAllOverlays(), "Show all hidden windows.");
    }
}