using System.Collections.Generic;
using Godot;

namespace Betauer.Input {
    public class DirectionInput : IActionUpdate {
        public static readonly ISet<KeyList> CursorLateralPositive = new HashSet<KeyList> { KeyList.Right };
        public static readonly ISet<KeyList> CursorLateralNegative = new HashSet<KeyList> { KeyList.Left };
        public static readonly ISet<KeyList> CursorVerticalPositive = new HashSet<KeyList> { KeyList.Down };
        public static readonly ISet<KeyList> CursorVerticalNegative = new HashSet<KeyList> { KeyList.Up };

        public static readonly ISet<JoystickList> DPadLateralPositive = new HashSet<JoystickList>
            { JoystickList.DpadRight };

        public static readonly ISet<JoystickList> DPadLateralNegative = new HashSet<JoystickList>
            { JoystickList.DpadLeft };

        public static readonly ISet<JoystickList> DPadVerticalPositive = new HashSet<JoystickList>
            { JoystickList.DpadDown };

        public static readonly ISet<JoystickList> DPadVerticalNegative = new HashSet<JoystickList>
            { JoystickList.DpadUp };

        private ISet<int> _positiveKeys;
        private ISet<int> _negativeKeys;
        private ISet<int> _positiveButtons;
        private ISet<int> _negativeButtons;
        private JoystickList _axis;

        private readonly IKeyboardOrController _isKeyboardOrController;

        private float _inputPositiveKey = 0f;
        private float _inputNegativeKey = 0f;
        private float _inputPositiveButton = 0f;
        private float _inputNegativeButton = 0f;
        private float _inputTotalKey = 0f;
        private float _inputTotalButton = 0f;
        private float _axisValue = 0f;
        public int DeviceId = -1;
        private bool _buttons = false;

        public float AxisDeadZone = 0.5f;

        public float Strength =>
            _isKeyboardOrController.IsUsingKeyboard ? _inputTotalKey : _buttons ? _inputTotalButton : _axisValue;

        public DirectionInput(string name, IKeyboardOrController isKeyboardOrController, int deviceId) : base(name) {
            _isKeyboardOrController = isKeyboardOrController;
            DeviceId = deviceId;
            ClearConfig();
        }

        public override bool Update(EventWrapper w) {
            if (!Enabled) return false;
            if (CheckAxis(w)) {
                _isKeyboardOrController.IsUsingKeyboard = false;
                _buttons = false;
                return true;
            }

            if (CheckButtons(w)) {
                _isKeyboardOrController.IsUsingKeyboard = false;
                _buttons = true;
                _inputTotalButton = _inputPositiveButton - _inputNegativeButton;
                return true;
            }

            if (CheckKeys(w)) {
                _isKeyboardOrController.IsUsingKeyboard = true;
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
            if (w.IsAxis((int)_axis, DeviceId)) {
                _axisValue = w.GetStrength(AxisDeadZone);
                return true;
            }

            return false;
        }

        public DirectionInput ClearConfig() {
            _positiveKeys = new HashSet<int>();
            _negativeKeys = new HashSet<int>();
            _positiveButtons = new HashSet<int>();
            _negativeButtons = new HashSet<int>();
            _axis = JoystickList.InvalidOption;
            return this;
        }

        public DirectionInput SetDevice(int deviceId) {
            DeviceId = deviceId;
            return this;
        }

        public DirectionInput SetAllDevices() {
            DeviceId = -1;
            return this;
        }

        public DirectionInput AddKeyPositive(KeyList positive) {
            _positiveKeys.Add((int)positive);
            return this;
        }

        public DirectionInput AddKeyNegative(KeyList negative) {
            _negativeKeys.Add((int)negative);
            return this;
        }

        public DirectionInput AddButtonPositive(JoystickList positive) {
            _positiveButtons.Add((int)positive);
            return this;
        }

        public DirectionInput AddButtonNegative(JoystickList negative) {
            _negativeButtons.Add((int)negative);
            return this;
        }

        public DirectionInput AddLateralCursorKeys() {
            foreach (var i in CursorLateralPositive) AddKeyPositive(i);
            foreach (var i in CursorLateralNegative) AddKeyNegative(i);
            return this;
        }

        public DirectionInput AddVerticalCursorKeys() {
            foreach (var i in CursorVerticalPositive) AddKeyPositive(i);
            foreach (var i in CursorVerticalNegative) AddKeyNegative(i);
            return this;
        }

        public DirectionInput AddLateralDPadButtons() {
            foreach (var i in DPadLateralPositive) AddButtonPositive(i);
            foreach (var i in DPadLateralNegative) AddButtonNegative(i);
            return this;
        }

        public DirectionInput AddVerticalDPadButtons() {
            foreach (var i in DPadVerticalPositive) AddButtonPositive(i);
            foreach (var i in DPadVerticalNegative) AddButtonNegative(i);
            return this;
        }


        public DirectionInput ConfigureAxis(JoystickList axis) {
            _axis = axis;
            return this;
        }
    }
}