using System.Collections.Generic;
using Godot;

namespace Betauer.Input {
    public class ActionState : BaseAction {
        private ISet<int> _buttons = new HashSet<int>();
        private ISet<uint> _keys = new HashSet<uint>();

        public bool Pressed => Godot.Input.IsActionPressed(Name);
        public bool JustPressed => Godot.Input.IsActionJustPressed(Name);
        public bool JustReleased => Godot.Input.IsActionJustReleased(Name);

        public bool IsEventPressed(InputEvent e, bool echo = false) {
            return ActionPressed(Name, e, echo);
        }

        private readonly IKeyboardOrController _isKeyboardOrController;
        private readonly int _deviceId = -1;
        public readonly string Name;

        public ActionState(string name, IKeyboardOrController isKeyboardOrController, int deviceId) {
            Name = name;
            _isKeyboardOrController = isKeyboardOrController;
            _deviceId = deviceId;
        }

        public ActionState Build() {
            if (InputMap.HasAction(Name)) InputMap.EraseAction(Name);
            InputMap.AddAction(Name);
            CreateInputEvents().ForEach((e) => InputMap.ActionAddEvent(Name, e));
            return this;
        }

        private List<InputEvent> CreateInputEvents() {
            List<InputEvent> events = new List<InputEvent>();
            foreach (var key in _keys) {
                var e = new InputEventKey();
                e.Scancode = key;
                events.Add(e);
            }
            foreach (var button in _buttons) {
                var e = new InputEventJoypadButton();
                e.ButtonIndex = button;
                events.Add(e);
            }
            return events;
        }

        public ActionState ClearConfig() {
            _buttons = new HashSet<int>();
            _keys = new HashSet<uint>();
            return this;
        }

        public ActionState AddKey(KeyList key) {
            _keys.Add((uint)key);
            return this;
        }

        public ActionState AddButton(JoystickList button) {
            _buttons.Add((int)button);
            return this;
        }
    }
}