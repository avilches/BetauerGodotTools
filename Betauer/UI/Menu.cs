using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Betauer.UI {
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

        /*
         * Rebuild the menu ensures disabled buttons are not selectable when using previous-next
         * wrap true = link the first and last buttons
         */
        public ActionMenu Refresh(bool wrap = true) {
            ActionButton first = null;
            ActionButton last = null;
            foreach (var child in _holder.GetChildren()) {
                _holder.RemoveChild(child as Node);
            }
            foreach (var actionButton in Buttons) {
                if (actionButton.Disabled) {
                    actionButton.FocusMode = Control.FocusModeEnum.None;
                    _holder.AddChild(actionButton);
                    last = actionButton;
                } else {
                    if (first == null) {
                        Focus(actionButton);
                        first = actionButton;
                    }
                    _holder.AddChild(actionButton);
                    last = actionButton;
                    actionButton.FocusMode = Control.FocusModeEnum.All;
                }
            }
            if (wrap && first != null && last != null && first != last) {
                if (_holder is VBoxContainer) {
                    first.FocusNeighbourTop = "../" + last.Name;
                    last.FocusNeighbourBottom = "../" + first.Name;
                } else if (_holder is HBoxContainer) {
                    first.FocusNeighbourLeft = "../" + last.Name;
                    last.FocusNeighbourRight = "../" + first.Name;
                }
            }
            return this;
        }

        public async Task Focus(ActionButton button) {
            await _holder.GetTree().AwaitIdleFrame();
            button.GrabFocus();
        }
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