using System.Collections.Generic;
using Godot;

namespace Betauer.Tools.Input {
    public class ActionState: IActionUpdate {
        private ISet<int> _buttons;
        private ISet<int> _keys;

        public bool Pressed { get; private set; } = false;
        public bool JustPressed { get; private set; } = false;
        public bool JustReleased { get; private set; } = false;

        private readonly PlayerActions _playerActions;
        private readonly int _deviceId = -1;
        public string Name;

        public ActionState(string name, PlayerActions playerActions, int deviceId) {
            Name = name;
            _playerActions = playerActions;
            _deviceId = deviceId;
        }

        public override bool Update(EventWrapper w) {
            if (!Enabled) return false;
            if (w.IsKey(_keys) || w.IsButton(_buttons, _deviceId)) {
                _playerActions.IsUsingKeyboard = w.IsAnyKey();
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

        public override void ClearJustState() {
            JustPressed = JustReleased = false;
        }

        public ActionState Configure(KeyList key, JoystickList button) {
            return Configure(new HashSet<int>{(int)key}, new HashSet<int>{(int)button});
        }

        public ActionState Configure(ISet<int> keys, ISet<int> buttons) {
            _keys = keys;
            _buttons = buttons;
            return this;
        }

    }
}