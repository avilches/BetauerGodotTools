using System.Collections.Generic;
using Godot;

namespace Tools.Input {
    public class DirectionInput : IActionUpdate {
        public static readonly ISet<int> Empty = new HashSet<int>();

        public static readonly ISet<int> CursorLateralPositive = new HashSet<int>{ (int)KeyList.Right };
        public static readonly ISet<int> CursorLateralNegative = new HashSet<int>{ (int)KeyList.Left };
        public static readonly ISet<int> DpadLateralPositive = new HashSet<int>{ (int)JoystickList.DpadRight };
        public static readonly ISet<int> DpadLateralNegative = new HashSet<int>{ (int)JoystickList.DpadLeft };

        public static readonly ISet<int> CursorVerticalPositive = new HashSet<int>{ (int)KeyList.Down };
        public static readonly ISet<int> CursorVerticalNegative = new HashSet<int>{ (int)KeyList.Up };
        public static readonly ISet<int> DpadVerticalPositive = new HashSet<int>{ (int)JoystickList.DpadDown };
        public static readonly ISet<int> DpadVerticalNegative = new HashSet<int>{ (int)JoystickList.DpadUp };

        private ISet<int> _positiveKeys;
        private ISet<int> _negativeKeys;
        private ISet<int> _positiveButtons;
        private ISet<int> _negativeButtons;
        private JoystickList _axis;
        
        private readonly PlayerActions _playerActions;

        private float _inputPositiveKey = 0f;
        private float _inputNegativeKey = 0f;
        private float _inputPositiveButton = 0f;
        private float _inputNegativeButton = 0f;
        private float _inputTotalKey = 0f;
        private float _inputTotalButton = 0f;
        private float _axisValue = 0f;
        public int DeviceId = -1;
        private bool _buttons = false;

        public string Name;
        public float AxisDeadZone = 0.5f;
        public float Strength => _playerActions.IsUsingKeyboard ? _inputTotalKey : _buttons?_inputTotalButton:_axisValue;

        public DirectionInput(string name, PlayerActions playerActions, int deviceId) {
            Name = name;
            _playerActions = playerActions;
            DeviceId = deviceId;

            ClearConfig();
        }

        public override bool Update(EventWrapper w) {
            if (!Enabled) return false;
            if (CheckAxis(w)) {
                _playerActions.IsUsingKeyboard = false;
                _buttons = false;
                return true;
            }

            if (CheckButtons(w)) {
                _playerActions.IsUsingKeyboard = false;
                _buttons = true;
                _inputTotalButton = _inputPositiveButton - _inputNegativeButton;
                return true;
            }

            if (CheckKeys(w)) {
                _playerActions.IsUsingKeyboard = true;
                _inputTotalKey = _inputPositiveKey - _inputNegativeKey;
                return true;
            }

            return false;
        }

        public override void ClearJustState() {
        }

        private bool CheckKeys(EventWrapper w) {
            if (w.IsKey(_positiveKeys)) {
                _inputPositiveKey = w.Pressed ? w.GetStrength() : 0;
                return true;
            } else if (w.IsKey(_negativeKeys)) {
                _inputNegativeKey = w.Pressed ? w.GetStrength() : 0;
                return true;
            }

            return false;
        }

        private bool CheckButtons(EventWrapper w) {
            if (w.IsButton(_positiveButtons, DeviceId)) {
                _inputPositiveButton = w.Pressed ? w.GetStrength() : 0;
                return true;
            } else if (w.IsButton(_negativeButtons, DeviceId)) {
                _inputNegativeButton = w.Pressed ? w.GetStrength() : 0;
                return true;
            }

            return false;
        }

        private bool CheckAxis(EventWrapper w) {
            if (w.IsAxis((int) _axis, DeviceId)) {
                _axisValue = w.GetStrength(AxisDeadZone);
                return true;
            }

            return false;
        }

        public DirectionInput ClearConfig() {
            _positiveKeys = Empty;
            _negativeKeys = Empty;
            _positiveButtons = Empty;
            _negativeButtons = Empty;
            _axis = JoystickList.InvalidOption;
            return this;
        }

        public DirectionInput ChangeDevice(int deviceId) {
            this.DeviceId = deviceId;
            return this;
        }

        public DirectionInput ConfigureKeys(ISet<int> positives, ISet<int> negatives) {
            _positiveKeys = positives;
            _negativeKeys = negatives;
            return this;
        }

        public DirectionInput ConfigureButtons(ISet<int> positives, ISet<int> negatives) {
            _positiveButtons = positives;
            _negativeButtons = negatives;
            return this;
        }

        public DirectionInput ConfigureAxis(JoystickList axis) {
            _axis = axis;
            return this;
        }

    }
}