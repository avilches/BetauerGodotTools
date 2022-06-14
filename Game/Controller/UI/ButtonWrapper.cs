using System;
using Betauer;
using Betauer.Animation;
using Godot;

namespace Veronenger.Game.Controller.UI {
    public class ButtonWrapper : Button {
        public class Context {
            public ButtonWrapper Button { get; }

            public Context(ButtonWrapper button) {
                Button = button;
            }
        }

        public class InputEventContext : Context {
            public InputEvent InputEvent { get; }

            public InputEventContext(ButtonWrapper button, InputEvent @event) : base(button) {
                InputEvent = @event;
            }
        }

        private readonly ControlRestorer _saver;
        private Action<Context>? _onPressedAction;
        private Func<InputEventContext, bool>? _onInputEvent;
        private Action? _onFocusEntered;
        private Action? _onFocusExited;

        // TODO: i18n
        internal ButtonWrapper() {
            _saver = new ControlRestorer(this);
            Connect(SignalConstants.BaseButton_PressedSignal, this, nameof(_GodotPressedSignal));
        }

        public void Save() => _saver.Save();
        public void Restore() => _saver.Restore();

        public override void _Input(InputEvent @event) {
            // It takes into account if the Root.GuiDisableInput = true
            if (_onInputEvent == null || GetFocusOwner() != this || Disabled) return;
            if (_onInputEvent(new InputEventContext(this, @event))) {
                GetTree().SetInputAsHandled();
            }
        }

        private void _GodotPressedSignal() => _onPressedAction?.Invoke(new Context(this));
        private void _GodotFocusEnteredSignal() => _onFocusEntered?.Invoke();
        private void _GodotFocusExitedSignal() => _onFocusExited?.Invoke();

        public ButtonWrapper OnPressed(Action onPressedAction) {
            _onPressedAction = context => onPressedAction();
            return this;
        }

        public ButtonWrapper OnPressed(Action<bool> onPressedAction) {
            _onPressedAction = context => onPressedAction(context.Button.Pressed);
            return this;
        }

        public ButtonWrapper OnPressed(Action<Context> onPressedAction) {
            _onPressedAction = onPressedAction;
            return this;
        }

        public ButtonWrapper OnInputEvent(Func<InputEventContext, bool>? onInputEvent) {
            _onInputEvent = onInputEvent;
            return this;
        }

        public ButtonWrapper OnFocusEntered(Action onFocus) {
            if (_onFocusEntered == null) 
                Connect(SignalConstants.Control_FocusEnteredSignal, this, nameof(_GodotFocusEnteredSignal));
            _onFocusEntered = onFocus;
            return this;
        }

        public ButtonWrapper OnFocusExited(Action onFocus) {
            if (_onFocusExited == null) 
                Connect(SignalConstants.Control_FocusExitedSignal, this, nameof(_GodotFocusExitedSignal));
            _onFocusExited = onFocus;
            return this;
        }
    }
}