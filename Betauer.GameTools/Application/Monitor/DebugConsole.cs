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
            void Execute(CommandInput input);
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
            private readonly Action<CommandInput> _execute;

            public Command(string name, Action<CommandInput> execute, string shortHelp, string? longHelp) {
                Name = name;
                ShortHelp = shortHelp;
                LongHelp = longHelp;
                _execute = execute;
            }

            public void Execute(CommandInput input) {
                _execute(input);
            }
        }
        
        public class ConditionalCommand : ICommand {
            public string Name { get; }
            public string ShortHelp { get; }
            public string? LongHelp { get; }
            public string? ErrorMessage { get; }
            private readonly List<(Func<CommandInput, bool>, Action<CommandInput>)> _execute = new();
            private readonly DebugConsole _debugConsole;

            public ConditionalCommand(DebugConsole debugConsole, string name, string shortHelp, string? longHelp, string? errorMessage) {
                _debugConsole = debugConsole;
                Name = name;
                ShortHelp = shortHelp;
                LongHelp = longHelp;
                ErrorMessage = errorMessage;
            }

            public ConditionalCommand WithNoArguments(Action execute) {
                _execute.Add((input => input.Arguments.Length == 0, (_) => execute()));
                return this;
            }

            public ConditionalCommand ArgumentIsInteger(Action<CommandInput> execute) {
                _execute.Add((input => input.Arguments.Length > 0 && input.Arguments[0].IsValidInteger(), execute));
                return this;
            }

            public ConditionalCommand ArgumentIsFloat(Action<CommandInput> execute) {
                _execute.Add((input => input.Arguments.Length > 0 && input.Arguments[0].IsValidFloat(), execute));
                return this;
            }

            public ConditionalCommand ArgumentIsPresent(Action<CommandInput> execute) {
                _execute.Add((input => input.Arguments.Length > 0, execute));
                return this;
            }

            public ConditionalCommand ArgumentMatches(string match, Action<CommandInput> execute) {
                _execute.Add((input => input.Arguments.Length > 0 && string.Equals(input.Arguments[0], match, StringComparison.CurrentCultureIgnoreCase), execute));
                return this;
            }


            public void Execute(CommandInput input) {
                var found = false;
                foreach (var valueTuple in _execute) {
                    found = valueTuple.Item1(input);
                    if (found) {
                        valueTuple.Item2(input);
                        return;
                    }
                }
                _debugConsole.WriteLine($"Error: {(string.IsNullOrWhiteSpace(ErrorMessage) ? "Error: wrong argument/s" : ErrorMessage)}");
                _debugConsole.WriteLine(string.IsNullOrWhiteSpace(LongHelp) ? ShortHelp : LongHelp);
            }

            public DebugConsole End() {
                return _debugConsole;
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
        private bool IsAutoCompleting => _caretAutoCompleting > 0;
        private readonly List<string> _sortedCommandList = new();        
        private int _caretAutoCompleting = -1;

        public DebugOverlayManager DebugOverlayManager { get; }
        public readonly Dictionary<string, ICommand> Commands = new();        
        public readonly RichTextLabel ConsoleOutput = new();
        public readonly LineEdit Prompt = new();
        public readonly List<string> History = new();

        public DebugConsole(DebugOverlayManager debugOverlayManager) {
            DebugOverlayManager = debugOverlayManager;
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

        public DebugConsole WriteLine(string? text = null) {
            if (text != null) ConsoleOutput.AppendBbcode(text);
            ConsoleOutput.Newline();
            return this;
        }
        
        public DebugConsole ClearConsole() {
            ConsoleOutput.Clear();
            ConsoleOutput.PushColor(new Color(0.8f, 0.8f, 0.8f));
            return this;
        }
        
        public DebugConsole AddCommand(ICommand command) {
            Commands[command.Name.ToLower()] = command;
            _sortedCommandList.Add(command.Name.ToLower());
            _sortedCommandList.Sort();
            return this;
        }

        public DebugConsole Sleep() {
            SetProcessInput(false);
            return this;
        }

        public DebugConsole Enable(bool enable = true) {
            Visible = enable;
            if (enable) {
                Prompt.GrabFocus();
            }
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
                        margin.Name = "MarginContainer";
                        margin.AnchorRight = 1;
                        margin.AnchorBottom = 1;
                        margin.SetMargin(10, 10, 10, 10);
                    })
                    .Child<VBoxContainer>()
                        .Config(box => {
                            // Full rect
                            box.Name = "VBoxContainer";
                            box.AnchorRight = 1;
                            box.AnchorBottom = 1;
                        })
                        .Child(ConsoleOutput)
                            .Config(text => {
                                text.Name = nameof(ConsoleOutput);
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
                                    label.Name = "Prompt";
                                    label.Text = ">";
                                })
                            .End()
                            .Child(Prompt)
                                .Config(prompt => {
                                    prompt.Name = nameof(Prompt);
                                    var style = new StyleBoxFlat() {
                                        BgColor = DebugOverlay.ColorInvisible,
                                        BorderWidthTop = 0,
                                        BorderWidthRight = 0,
                                        BorderWidthBottom = 0,
                                        BorderWidthLeft = 0
                                    };
                                    prompt.AddStyleboxOverride("normal", style);
                                    prompt.AddStyleboxOverride("focus", style);
                                    prompt.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
                                    prompt.CaretBlink = true;
                                    prompt.CaretBlinkSpeed = 0.250f;
                                    prompt.GrabFocus();
                                    prompt.OnTextChanged((_) => _caretAutoCompleting = -1);         
                                })
                            .End()
                            .Child<HSlider>()
                                .Config(slider => {
                                    slider.Name = "OpacitySlider";
                                    slider.Editable = true;
                                    slider.HintTooltip = "Opacity";
                                    slider.MinSize = new Vector2(50, 5);
                                    slider.Value = InitialTransparentBackground * 25f;
                                    slider.OnValueChanged((value) => {
                                        SelfModulate = new Color(1, 1, 1, value / 25f);
                                        Prompt.GrabFocus();
                                    });
                                    slider.MaxValue = 25f;
                                    slider.MinValue = 0f;
                                })
                            .End()
                        .End()
                    .End()
                .End();
            
            SelfModulate = new Color(1, 1, 1, InitialTransparentBackground);
            Layout = LayoutEnum.DownThird;
            ClearConsole();
        }

        private void SetConsoleInputText(string text) {
            Prompt.Text = text;
            Prompt.CaretPosition = text.Length;
        }

        private void OnConsoleInputKeyEvent(InputEventKey eventKey) {
            if (eventKey.IsKeyJustPressed(KeyList.Enter) || eventKey.IsKeyJustPressed(KeyList.KpEnter)) {
                OnTextEntered(Prompt.Text);
                
            } else if (eventKey.IsKeyJustPressed(KeyList.Up) && (eventKey.HasAlt() || eventKey.HasMeta())) {
                var newState = Math.Min((int)_layout + 1, DebugConsoleLayoutEnumSize - 1);
                Layout = (LayoutEnum)newState;
                GetTree().SetInputAsHandled();
                
            } else if (eventKey.IsKeyJustPressed(KeyList.Down) && (eventKey.HasAlt() || eventKey.HasMeta())) {
                var newState = Math.Max((int)_layout - 1, 0);
                Layout = (LayoutEnum)newState;
                GetTree().SetInputAsHandled();
                
            } else if (eventKey.IsKeyJustPressed(KeyList.Up)) {
                OnHistoryUp();
            } else if (eventKey.IsKeyJustPressed(KeyList.Down)) {
                OnHistoryDown();
            } else if (eventKey.IsKeyJustPressed(KeyList.Tab)) {
                OnAutocomplete(eventKey.HasShift() ? -1 : 1);
            }
        }

        public override void _Input(InputEvent input) {
            if (!Visible) {
                Disable();
            } else if (Prompt.HasFocus() && input is InputEventKey eventKey) {
                OnConsoleInputKeyEvent(eventKey);
            } else if (input.IsLeftClickJustPressed() && input.IsMouseInside(ConsoleOutput)) {
                Prompt.GrabFocus();
                GetTree().SetInputAsHandled();
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
                    command.Execute(_commandInput);
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
                _historyBuffer = Prompt.Text;
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

        private void OnAutocomplete(int next) {
            if (!IsAutoCompleting) _caretAutoCompleting = Prompt.CaretPosition;
            var caret = _caretAutoCompleting;            
            if (caret != 0 && Prompt.Text.IndexOf(" ", StringComparison.Ordinal) <= -1) {
                var promptText = Prompt.Text;
                var autocompleteFrom = promptText[..caret].ToLower();
                var matches = _sortedCommandList
                    .Where(command => command.StartsWith(autocompleteFrom))
                    .ToList();
                if (matches.Count > 0) {
                    var found = matches.IndexOf(promptText);
                    found =(found + next).Mod(matches.Count);
                    Prompt.Text = matches[found];
                    Prompt.CaretPosition = Prompt.Text.Length;
                }
            }
            GetTree().SetInputAsHandled();
        }
    }
}