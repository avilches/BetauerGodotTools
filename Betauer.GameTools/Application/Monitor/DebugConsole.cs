using System;
using System.Collections.Generic;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor {
    public class DebugConsole : Panel {

        public interface ICommand {
            string Name { get; }
            string ShortHelp { get; }
            string? LongHelp { get; }
            void Execute(CommandInput input, RichTextLabel output);
        }

        public class CommandInput {
            public string Raw { get; internal set; }
            public string Name { get; internal set; }
            public string ArgumentString { get; internal set; }
            public string[] Arguments { get; internal set; }
        }
        
        public class Command : ICommand {
            public string Name { get; }
            public string ShortHelp { get; }
            public string? LongHelp { get; }
            private readonly Action<CommandInput, RichTextLabel> _execute;

            public Command(string name, Action<CommandInput, RichTextLabel> execute, string shortHelp, string? longHelp) {
                Name = name;
                ShortHelp = shortHelp;
                LongHelp = longHelp;
                _execute = execute;
            }

            public void Execute(CommandInput input, RichTextLabel output) {
                _execute(input, output);
            }
        }
        
        private static readonly int DebugConsoleLayoutEnumSize = Enum.GetNames(typeof(LayoutEnum)).Length;
        public enum LayoutEnum {
            UpQuarter, UpThird, UpHalf, DownHalf, DownThird, DownQuarter
        }
        private int _historyPos = -1;
        private string? _historyBuffer = null;
        private bool IsBrowsingHistory => _historyPos > -1;
        private LayoutEnum _layout = LayoutEnum.DownThird;
        private readonly CommandInput _commandInput = new();

        public readonly Dictionary<string, ICommand> Commands = new();
        
        public readonly RichTextLabel ConsoleOutput = new();
        public readonly LineEdit ConsoleInput = new();
        public readonly List<string> History = new();

        public DebugConsole() {
            this.CreateCommand("help", OnHelp, "Show this help.", @"Usage:
    help           : List all available commands.
    help <command> : Show help about a specific command.");
            this.AddEngineTimeScale();
            this.AddEngineTargetFps();
            this.AddQuit();
        }

        public LayoutEnum Layout {
            get => _layout;
            set {
                _layout = value;
                AnchorLeft = 0;
                AcceptEvent();
                AnchorRight = 1f;
                if (_layout == LayoutEnum.UpQuarter) {
                    AnchorTop = 0;
                    AnchorBottom = 0.25f;
                } else if (_layout == LayoutEnum.UpThird) {
                    AnchorTop = 0;
                    AnchorBottom = 0.33f;
                } else if (_layout == LayoutEnum.UpHalf) {
                    AnchorTop = 0;
                    AnchorBottom = 0.75f;
                } else if (_layout == LayoutEnum.DownHalf) {
                    AnchorTop = 0.75f;
                    AnchorBottom = 1;
                } else if (_layout == LayoutEnum.DownThird) {
                    AnchorTop = 0.66f;
                    AnchorBottom = 1;
                } else if (_layout == LayoutEnum.DownQuarter) {
                    AnchorTop = 0.75f;
                    AnchorBottom = 1;
                }
            }
        }

        private DebugConsole WriteLine(string? text = null) {
            if (text != null) ConsoleOutput.AppendBbcode(text);
            ConsoleOutput.Newline();
            return this;
        }
        private void OnHelp(string[] arguments, RichTextLabel _) {
            if (arguments.Length == 0) {
                WriteLine();
                WriteLine("Press Shift+Up or Shift+Down to change the console size.");
                WriteLine();
                WriteLine("Available commands:");
                WriteLine();
                foreach (var command in Commands.Values) {
                    WriteLine($"    [color=#ffffff]{command.Name}[/color]: {command.ShortHelp}");
                }
                WriteLine();
                WriteLine("Type `help <command>` to get more info about a command");
            } else {
                var commandName = arguments[0];
                if (Commands.TryGetValue(commandName.ToLower(), out var command)) {
                    if (command.LongHelp != null) {
                        WriteLine($"Help for command `{command.Name}`:");
                        WriteLine(command.LongHelp);
                    } else {
                        WriteLine($"Command `{commandName}` doesn't have help.");
                    }
                } else {
                    WriteLine($"Error getting help: command `{commandName}` not found.");
                }
            }
        }
        
        public DebugConsole AddCommand(ICommand command) {
            Commands[command.Name.ToLower()] = command;
            return this;
        }
        public DebugConsole Awake() {
            SetProcessInput(true);
            return this;
        }

        public DebugConsole Sleep() {
            SetProcessInput(false);
            return this;
        }

        public DebugConsole Enable(bool enable = true) {
            Visible = enable;
            if (enable) ConsoleInput.GrabFocus();
            SetProcessInput(enable);
            return this;
        }

        public DebugConsole Disable() {
            return Enable(false);
        }

        public override void _Ready() {
            this.NodeBuilder()
                .Child<MarginContainer>()
                    .Config(margin => {
                        // Full rect
                        margin.AnchorRight = 1;
                        margin.AnchorBottom = 1;
                        margin.SetMargin(10, 10, 10, 10);
                    })
                    .Child<VBoxContainer>()
                        .Config(box => {
                            // Full rect
                            box.AnchorRight = 1;
                            box.AnchorBottom = 1;
                        })
                        .Child(ConsoleOutput)
                            .Config(text => {
                                text.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
                                text.SizeFlagsVertical = (int)SizeFlags.ExpandFill;
                                text.BbcodeEnabled = true;
                                text.SelectionEnabled = true;
                                text.ScrollFollowing = true;
                                text.PushColor(new Color(0.75f, 0.75f, 0.75f));
                            })
                        .End()
                        .Child<HBoxContainer>()
                            .Child<Label>()
                                .Config(label => {
                                    label.Text = ">";
                                })
                            .End()
                            .Child(ConsoleInput)
                                .Config(text => {
                                    var style = new StyleBoxFlat() {
                                        BgColor = DebugOverlay.ColorInvisible,
                                        BorderWidthTop = 0,
                                        BorderWidthRight = 0,
                                        BorderWidthBottom = 0,
                                        BorderWidthLeft = 0
                                    };
                                    text.AddStyleboxOverride("normal", style);
                                    text.AddStyleboxOverride("focus", style);
                                    text.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
                                    text.CaretBlink = true;
                                    text.CaretBlinkSpeed = 0.250f;
                                    text.SetFontColor(new Color(0.75f, 0.75f, 0.75f));
                                    text.GrabFocus();
                                })
                            .End()
                        .End()
                    .End()
                .End();
            
            SelfModulate = new Color(1, 1, 1, 0.8f);
            Layout = LayoutEnum.DownThird;
        }

        private void SetConsoleInputText(string text) {
            ConsoleInput.Text = text;
            ConsoleInput.CaretPosition = text.Length;
        }

        private void OnConsoleInputKeyEvent(InputEventKey eventKey) {
            if (eventKey.IsKeyJustPressed(KeyList.Enter) || eventKey.IsKeyJustPressed(KeyList.KpEnter)) {
                OnTextEntered(ConsoleInput.Text);
            } else if (eventKey.IsKeyJustPressed(KeyList.Up) && eventKey.HasShift()) {
                var newState = Math.Max((int)_layout - 1, 0);
                Layout = (LayoutEnum)newState;
            } else if (eventKey.IsKeyJustPressed(KeyList.Down) && eventKey.HasShift()) {
                var newState = Math.Min((int)_layout + 1, DebugConsoleLayoutEnumSize - 1);
                Layout = (LayoutEnum)newState;
            } else if (eventKey.IsKeyJustPressed(KeyList.Up)) OnHistoryUp();
            else if (eventKey.IsKeyJustPressed(KeyList.Down)) OnHistoryDown();
        }

        public override void _Input(InputEvent @event) {
            if (!Visible) {
                SetProcessInput(false);
            } else if (ConsoleInput.HasFocus() && @event is InputEventKey eventKey) {
                OnConsoleInputKeyEvent(eventKey);
            }
        }
        
        private void OnTextEntered(string text) {
            if (!string.IsNullOrWhiteSpace(text)) {
                text = text.Trim();
                if (History.Count == 0 || History[^1] != text) History.Add(text);
                if (History.Count > 1100) History.RemoveRange(0, 100);
                _historyPos = -1;
                _historyBuffer = null;
                WriteLine($"> {text}");
                var pos = text.IndexOf(" ", StringComparison.Ordinal);
                var commandName = pos > 0 ? text[..pos] : text;
                if (Commands.TryGetValue(commandName.ToLower(), out var command)) {
                    var argumentString = pos > 0 ? text[(pos+1)..text.Length] : "";
                    var arguments = argumentString.Length > 0 ? argumentString.Split(" ") : Array.Empty<string>();
                    _commandInput.Raw = text;
                    _commandInput.Name = commandName;
                    _commandInput.ArgumentString = argumentString;
                    _commandInput.Arguments = arguments;
                    command.Execute(_commandInput, ConsoleOutput);
                } else {
                    WriteLine(ErrorCommandNotFound(commandName));
                }
            } else {
                WriteLine(">");
            }
            SetConsoleInputText("");
            GetTree().SetInputAsHandled();
        }

        private static string ErrorCommandNotFound(string commandName) {
            return $"Command `{commandName}` not found. Type `help` to list all available commands.";
        }


        private void OnHistoryUp() {
            if (IsBrowsingHistory) {
                if (_historyPos > 0) {
                    _historyPos -= 1;
                    SetConsoleInputText(History[_historyPos]);
                }
            } else if (History.Count > 0) {
                // Start browsing
                _historyBuffer = ConsoleInput.Text;
                _historyPos = History.Count - 1;
                SetConsoleInputText(History[_historyPos]);
            }
            GetTree().SetInputAsHandled();
        }

        private void OnHistoryDown() {
            if (IsBrowsingHistory) {
                if (_historyPos == History.Count - 1) {
                    SetConsoleInputText(_historyBuffer!);
                    _historyPos = -1;
                    _historyBuffer = null;
                } else {
                    _historyPos += 1;
                    SetConsoleInputText(History[_historyPos]);
                }
            }
            GetTree().SetInputAsHandled();
        }
    }
}