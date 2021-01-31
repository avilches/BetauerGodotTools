using System.Collections.Generic;
using Godot;

namespace Betauer.Tools.Input {
    public class DirectionInput : IActionUpdate {
        public static readonly ISet<int> Empty = new HashSet<int>();
        public static readonly ISet<int> CursorPositive = new HashSet<int>{ (int)KeyList.Right };
        public static readonly ISet<int> CursorNegative = new HashSet<int>{ (int)KeyList.Left };

        private ISet<int> _positives = CursorPositive;
        private ISet<int> _negatives = CursorNegative;
        private JoystickList _axis = JoystickList.Axis0;
        private readonly PlayerActions _playerActions;

        private float _inputPositive = 0f;
        private float _inputNegative = 0f;
        private float _input = 0f;
        private float _axisValue = 0f;
        private readonly int _deviceId = -1;

        public string Name;
        public float AxisDeadZone = 0.5f;
        public float Strength => _playerActions.IsUsingKeyboard ? _input : _axisValue;

        public DirectionInput(string name, PlayerActions playerActions, int deviceId) {
            Name = name;
            _playerActions = playerActions;
            _deviceId = deviceId;
        }

        public override bool Update(EventWrapper w) {
            if (!Enabled) return false;
            if (LateralAxisMovement(w)) {
                _playerActions.IsUsingKeyboard = false;
                return true;
            }

            if (LateralKeyMovement(w)) {
                _playerActions.IsUsingKeyboard = w.IsAnyKey();
                _input = _inputPositive - _inputNegative;
                return true;
            }

            return false;
        }

        public override void ClearJustState() {
        }

        private bool LateralKeyMovement(EventWrapper w) {
            if (w.IsKey(_positives)) {
                _inputPositive = w.Pressed ? w.GetStrength() : 0;
                return true;
            } else if (w.IsKey(_negatives)) {
                _inputNegative = w.Pressed ? w.GetStrength() : 0;
                return true;
            }

            return false;
        }

        private bool LateralAxisMovement(EventWrapper w) {
            if (w.IsAxis((int) _axis, _deviceId)) {
                _axisValue = w.GetStrength(AxisDeadZone);
                return true;
            }

            return false;
        }

        public DirectionInput Configure(ISet<int> positives, ISet<int> negatives, JoystickList axis, float axisDeadZone) {
            AxisDeadZone = axisDeadZone;
            ConfigureKeys(positives, negatives);
            ConfigureAxis(axis);
            return this;
        }

        public DirectionInput ConfigureKeysOnly(ISet<int> positives, ISet<int> negatives) {
            ConfigureKeys(positives, negatives);
            _axis = JoystickList.InvalidOption;
            return this;
        }

        public DirectionInput ConfigureKeys(ISet<int> positives, ISet<int> negatives) {
            _positives = positives;
            _negatives = negatives;
            return this;
        }

        public DirectionInput ConfigureAxisOnly(JoystickList axis) {
            _positives = Empty;
            _negatives = Empty;
            ConfigureAxis(axis);
            return this;
        }

        public DirectionInput ConfigureAxis(JoystickList axis) {
            _axis = axis;
            return this;
        }

    }
}