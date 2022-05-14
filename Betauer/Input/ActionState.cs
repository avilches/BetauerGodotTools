using System.Collections.Generic;
using Godot;

namespace Betauer.Input {
    public class ActionState : BaseAction {
        public ISet<JoystickList> Buttons = new HashSet<JoystickList>();
        public ISet<KeyList> Keys = new HashSet<KeyList>();

        public bool Pressed => Godot.Input.IsActionPressed(Name);
        public bool JustPressed => Godot.Input.IsActionJustPressed(Name);
        public bool JustReleased => Godot.Input.IsActionJustReleased(Name);

        public override bool IsEventPressed(InputEvent e, bool echo = false) {
            return InputTools.EventIsAction(Name, e, echo);
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
            foreach (var key in Keys) {
                var e = new InputEventKey();
                e.Scancode = (uint)key;
                events.Add(e);
            }
            foreach (var button in Buttons) {
                var e = new InputEventJoypadButton();
                e.ButtonIndex = (int)button;
                events.Add(e);
            }
            return events;
        }

        public ActionState ClearConfig() {
            Buttons = new HashSet<JoystickList>();
            Keys = new HashSet<KeyList>();
            return this;
        }

        public ActionState AddKey(KeyList key) {
            Keys.Add(key);
            return this;
        }

        public ActionState AddButton(JoystickList button) {
            Buttons.Add(button);
            return this;
        }
    }
}