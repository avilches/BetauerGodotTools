using System;
using Betauer;
using Betauer.Animation;
using Godot;

namespace Veronenger.Game.Controller.UI {
    public class ActionButton : Button {
        public class Context {
            public ActionButton ActionButton { get; }

            public Context(ActionButton actionButton) {
                ActionButton = actionButton;
            }
        }

        public class InputEventContext : Context {
            public InputEvent InputEvent { get; }

            public InputEventContext(ActionButton actionButton, InputEvent @event) : base(actionButton) {
                InputEvent = @event;
            }
        }


        private readonly ControlRestorer _saver;
        public Action? Action;
        public Action<Context>? ActionWithContext;
        public Func<InputEventContext, bool>? ActionWithInputEventContext;

        // TODO: i18n
        internal ActionButton() {
            _saver = new ControlRestorer(this);
            Connect(GodotConstants.GODOT_SIGNAL_pressed, this, nameof(Execute));
        }

        public override void _Input(InputEvent @event) {
            // It takes into account if the Root.GuiDisableInput = true
            if (ActionWithInputEventContext != null && GetFocusOwner() == this && !Disabled) {
                if (ActionWithInputEventContext(new InputEventContext(this, @event))) {
                    GetTree().SetInputAsHandled();
                }
            }
        }

        public void Execute() {
            if (ActionWithContext != null) ActionWithContext(new Context( this));
            else Action?.Invoke();
        }

        public void Save() =>_saver.Save();
        public void Restore() => _saver.Restore();

        private Action _onFocusEntered;
        private void ExecuteOnFocusEntered() => _onFocusEntered?.Invoke();
        public void OnFocusEntered(Action onFocus) {
            Connect(GodotConstants.GODOT_SIGNAL_focus_entered, this, nameof(ExecuteOnFocusEntered));
            _onFocusEntered = onFocus;
        }

        private Action _onFocusExited;
        private void ExecuteOnFocusExited() => _onFocusExited?.Invoke();
        public void OnFocusExited(Action onFocus) {
            Connect(GodotConstants.GODOT_SIGNAL_focus_exited, this, nameof(ExecuteOnFocusExited));
            _onFocusExited = onFocus;
        }
    }
}
