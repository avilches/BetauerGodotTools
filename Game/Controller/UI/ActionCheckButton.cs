using System;
using Betauer;
using Betauer.Animation;
using Godot;

namespace Veronenger.Game.Controller.UI {
    public class ActionCheckButton : CheckButton {
        public class Context {
            public ActionCheckButton ActionCheckButton { get; }

            public bool Pressed => ActionCheckButton.Pressed;

            public Context(ActionCheckButton actionCheckButton) {
                ActionCheckButton = actionCheckButton;
            }
        }

        public class InputEventContext : Context {
            public InputEvent InputEvent { get; }

            public InputEventContext(ActionCheckButton actionCheckButton, InputEvent @event) : base(actionCheckButton) {
                InputEvent = @event;
            }
        }


        private readonly ControlRestorer _saver;
        public Action<bool>? Action;
        public Action<Context>? ActionWithContext;
        public Func<InputEventContext, bool>? ActionWithInputEventContext;

        // TODO: i18n
        internal ActionCheckButton() {
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
            else Action?.Invoke(Pressed);
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
