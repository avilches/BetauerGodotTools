using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.Signal;
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
        private const float InitialTransparentBackground = 0.8f;
        public enum LayoutEnum {
            UpQuarter = 5, 
            UpThird = 4, 
            UpHalf = 3, 
            DownHalf = 2,
            DownThird = 1,
            DownQuarter = 0
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
    [color=#ffffff]help          [/color] : List all available commands.
    [color=#ffffff]help <command>[/color] : Show help about a specific command.");
            this.AddEngineTimeScale();
            this.AddEngineTargetFps();
            this.AddClearConsole();
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
                    AnchorBottom = 0.5f;
                } else if (_layout == LayoutEnum.DownHalf) {
                    AnchorTop = 0.5f;
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
                var commands = Commands.Values.OrderBy(command => command.Name).ToList();
                var maxLength = commands.Max(command => command.Name.Length);
                WriteLine();
                WriteLine("Press [color=#ffffff]Alt+Up[/color] / [color=#ffffff]Alt+Down[/color] to change the console size and position.");
                WriteLine();
                WriteLine("Commands:");
                WriteLine();
                foreach (var command in commands) {
                    WriteLine($"    [color=#ffffff]{command.Name.PadRight(maxLength)}[/color] : {command.ShortHelp}");
                }
                WriteLine();
                WriteLine("Type `help <command>` to get more info about a command.");
            } else {
                var commandName = arguments[0];
                if (Commands.TryGetValue(commandName.ToLower(), out var command)) {
                    if (command.LongHelp != null) {
                        WriteLine($"Help for command `{command.Name}`");
                        WriteLine(command.LongHelp);
                    } else {
                        WriteLine($"No help for command `{commandName}`.");
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
                                    text.GrabFocus();
                                })
                            .End()
                            .Child<HSlider>()
                                .Config(slider => {
                                    slider.Editable = true;
                                    slider.HintTooltip = "Opacity";
                                    slider.RectMinSize = new Vector2(50, 5);
                                    slider.Value = InitialTransparentBackground * 25f;
                                    slider.OnValueChanged((value) => SelfModulate = new Color(1, 1, 1, value/25f));
                                    slider.MaxValue = 25f;
                                    slider.MinValue = 0f;
                                })
                            .End()
                        .End()
                    .End()
                .End();
            
            SelfModulate = new Color(1, 1, 1, InitialTransparentBackground);
            Layout = LayoutEnum.DownThird;
        }

        private void SetConsoleInputText(string text) {
            ConsoleInput.Text = text;
            ConsoleInput.CaretPosition = text.Length;
        }

        private void OnConsoleInputKeyEvent(InputEventKey eventKey) {
            if (eventKey.IsKeyJustPressed(KeyList.Enter) || eventKey.IsKeyJustPressed(KeyList.KpEnter)) {
                OnTextEntered(ConsoleInput.Text);
            } else if (eventKey.IsKeyJustPressed(KeyList.Up) && (eventKey.HasAlt() || eventKey.HasMeta())) {
                var newState = Math.Min((int)_layout + 1, DebugConsoleLayoutEnumSize - 1);
                Layout = (LayoutEnum)newState;
            } else if (eventKey.IsKeyJustPressed(KeyList.Down) && (eventKey.HasAlt() || eventKey.HasMeta())) {
                var newState = Math.Max((int)_layout - 1, 0);
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