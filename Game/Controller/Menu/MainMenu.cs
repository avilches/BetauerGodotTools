using System;
using System.Collections.Generic;
using System.Linq;
using Betauer;
using Godot;
using NUnit.Framework;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public class MainMenu : DiControl {
        [OnReady("MarginContainer/HBoxContainer/VBoxContainer/Menu")]
        private VBoxContainer _menuOptions;

        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(MainMenu));
        [Inject] public InputManager InputManager;
        [Inject] public GameManager GameManager;

        private SceneTree _sceneTree;
        private ActionMenu _actionMenu;
        private ActionButton _newGame;
        private ActionButton _continue;
        private ActionButton _options;
        private ActionButton _quit;

        public override void Ready() {
            _sceneTree = GetTree();
            BuildMenu();
        }

        public void BuildMenu() {
            _actionMenu = new ActionMenu(_menuOptions);
            _newGame = _actionMenu.CreateButton("New game", () => {
                _options.Disabled = true;
                _actionMenu.Build();
                GD.Print("New Game");
            });
            _continue = _actionMenu.CreateButton("Continue", () => GD.Print("Continue"));
            _options = _actionMenu.CreateButton("Options", () => GD.Print("Options"));
            _quit = _actionMenu.CreateButton("Quit", () => GameManager.Quit());
            _actionMenu.Build();
        }

        public class ActionMenu {
            private readonly Node _holder;

            public ActionMenu(Node holder) {
                _holder = holder;
            }

            public readonly List<ActionButton> Buttons = new List<ActionButton>();

            public ActionMenu Clear() {
                Buttons.Clear();
                return this;
            }

            public ActionButton CreateButton(string title, Action action) {
                ActionButton button = new ActionButton(action).SetAction(action);
                button.Name = $"B{Buttons.Count}";
                button.Text = title;
                Buttons.Add(button);
                return button;
            }

            public ActionMenu Build() {
                ActionButton first = null;
                ActionButton last = null;
                foreach (var child in _holder.GetChildren()) {
                    _holder.RemoveChild(child as Node);
                }
                foreach (var actionButton in Buttons) {
                    if (actionButton.Disabled) {
                        actionButton.FocusMode = FocusModeEnum.None;
                        _holder.AddChild(actionButton);
                        last = actionButton;
                    } else {
                        if (first == null) {
                            Focus(actionButton);
                            first = actionButton;
                        }
                        _holder.AddChild(actionButton);
                        last = actionButton;
                        actionButton.FocusMode = FocusModeEnum.All;

                    }
                }
                if (first != last) {
                    first.FocusNeighbourTop = "../" + last.Name;
                    last.FocusNeighbourBottom = "../" + first.Name;
                }
                return this;
            }

            public async void Focus(ActionButton button) {
                await _holder.GetTree().AwaitIdleFrame();
                button.GrabFocus();
            }
        }

        public override void _Process(float delta) {
            base._Process(delta);
        }

        public class ActionButton : DiButton {
            private Action _action;

            public ActionButton(Action action) {
                _action = action;
            }

            public override void Ready() {
                Connect("pressed", this, nameof(_Pressed));
            }

            public void _Pressed() {
                _action?.Invoke();
            }

            public ActionButton SetAction(Action action) {
                _action = action;
                return this;
            }

            public void Execute() {
                _Pressed();
            }

            public Node CreateDisabledButton() {
                var label = new Label();
                label.Text = Text;
                return label;
            }
        }
    }
}