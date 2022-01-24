using System.Collections.Generic;
using Godot;

namespace Betauer.Input {
    public class ActionState : IActionUpdate {
        private ISet<int> _buttons = new HashSet<int>();
        private ISet<int> _keys = new HashSet<int>();

        public bool Pressed { get; private set; } = false;
        public bool JustPressed { get; private set; } = false;
        public bool JustReleased { get; private set; } = false;

        private readonly IKeyboardOrController _isKeyboardOrController;
        private readonly int _deviceId = -1;

        public ActionState(string name, IKeyboardOrController isKeyboardOrController, int deviceId) : base(name) {
            Name = name;
            _isKeyboardOrController = isKeyboardOrController;
            _deviceId = deviceId;
        }

        public override bool Update(EventWrapper w) {
            if (!Enabled) return false;
            if (w.IsKey(_keys) || w.IsButton(_buttons, _deviceId)) {
                _isKeyboardOrController.IsUsingKeyboard = w.IsAnyKey();
                if (w.Pressed) {
                    JustPressed = !Pressed;
                    Pressed = true;
                    JustReleased = false;
                } else {
                    JustReleased = Pressed;
                    Pressed = JustPressed = false;
                }
                return true;
            }
            ClearJustState();
            return false;
        }

        public override void ClearState() {
            Pressed = false;
        }

        public override void ClearJustState() {
            JustPressed = JustReleased = false;
        }

        public ActionState ClearConfig() {
            _buttons = new HashSet<int>();
            _keys = new HashSet<int>();
            return this;
        }

        public ActionState AddKey(KeyList key) {
            _keys.Add((int)key);
            return this;
        }

        public ActionState AddButton(JoystickList button) {
            _buttons.Add((int)button);
            return this;
        }
    }
}