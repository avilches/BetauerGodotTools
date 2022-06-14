using System.Collections.Generic;
using Godot;

namespace Betauer.Input {
    public class LateralAction : DirectionalAction {
        public LateralAction(string negativeName, string positiveName,
            int deviceId) : base(negativeName, positiveName, deviceId) {
        }

        public bool IsActionPressed(InputEvent e, bool echo = false) {
            return IsRightEventPressed(e, echo) || IsLeftEventPressed(e, echo);
        }

        public bool IsRightEventPressed(InputEvent e, bool echo = false) {
            return InputTools.EventIsAction(PositiveName, e, echo);
        }

        public bool IsLeftEventPressed(InputEvent e, bool echo = false) {
            return InputTools.EventIsAction(NegativeName, e, echo);
        }
    }

    public class VerticalAction : DirectionalAction {
        public VerticalAction(string negativeName, string positiveName,
            int deviceId) : base(negativeName, positiveName, deviceId) {
        }
        
        public bool IsDownEventPressed(InputEvent e, bool echo = false) {
            return InputTools.EventIsAction(PositiveName, e, echo);
        }

        public bool IsUpEventPressed(InputEvent e, bool echo = false) {
            return InputTools.EventIsAction(NegativeName, e, echo);
        }
    }

    public abstract class DirectionalAction : BaseAction {
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

        private ISet<uint> _positiveKeys;
        private ISet<uint> _negativeKeys;
        private ISet<int> _positiveButtons;
        private ISet<int> _negativeButtons;
        private JoystickList _axis;

        public int DeviceId = -1;

        public readonly string NegativeName;
        public readonly string PositiveName;

        public float AxisDeadZone { get; private set; } = 0.5f;

        public float Strength => Godot.Input.GetAxis(NegativeName, PositiveName);

        public DirectionalAction(string negativeName, string positiveName,
            int deviceId) {
            NegativeName = negativeName;
            PositiveName = positiveName;
            DeviceId = deviceId;
            ClearConfig();
        }

        public override bool IsEventAction(InputEvent e, bool includeEchoEvents = false) {
            return IsPositiveEventPressed(e, includeEchoEvents) || IsNegativeEventPressed(e, includeEchoEvents);
        }

        public bool IsPositiveEventPressed(InputEvent e, bool echo = false) {
            return InputTools.EventIsAction(PositiveName, e, echo);
        }

        public bool IsNegativeEventPressed(InputEvent e, bool echo = false) {
            return InputTools.EventIsAction(NegativeName, e, echo);
        }

        public DirectionalAction Build() {
            if (InputMap.HasAction(NegativeName)) InputMap.EraseAction(NegativeName);
            if (InputMap.HasAction(PositiveName)) InputMap.EraseAction(PositiveName);
            InputMap.AddAction(NegativeName, AxisDeadZone);
            InputMap.AddAction(PositiveName, AxisDeadZone);
            CreateNegativeInputEvents().ForEach((e) => InputMap.ActionAddEvent(NegativeName, e));
            CreatePositiveInputEvents().ForEach((e) => InputMap.ActionAddEvent(PositiveName, e));
            return this;
        }

        public DirectionalAction Copy(DirectionalAction lateralMotion) {
            _negativeKeys = new HashSet<uint>(lateralMotion._negativeKeys);
            _negativeButtons = new HashSet<int>(lateralMotion._negativeButtons);
            _positiveKeys = new HashSet<uint>(lateralMotion._positiveKeys);
            _positiveButtons = new HashSet<int>(lateralMotion._positiveButtons);
            _axis = lateralMotion._axis;
            return this;
        }

        public List<InputEvent> CreateNegativeInputEvents() {
            List<InputEvent> events = new List<InputEvent>();
            foreach (var key in _negativeKeys) {
                var e = new InputEventKey();
                e.Scancode = key;
                events.Add(e);
            }
            foreach (var button in _negativeButtons) {
                var e = new InputEventJoypadButton();
                e.Device = DeviceId;
                e.ButtonIndex = button;
                events.Add(e);
            }

            var negative = new InputEventJoypadMotion();
            negative.Device = DeviceId;
            negative.Axis = (int)_axis;
            negative.AxisValue = -1;
            events.Add(negative);
            return events;
        }

        public List<InputEvent> CreatePositiveInputEvents() {
            List<InputEvent> events = new List<InputEvent>();
            foreach (var key in _positiveKeys) {
                var e = new InputEventKey();
                e.Scancode = key;
                events.Add(e);
            }
            foreach (var button in _positiveButtons) {
                var e = new InputEventJoypadButton();
                e.Device = DeviceId;
                e.ButtonIndex = button;
                events.Add(e);
            }

            var positive = new InputEventJoypadMotion();
            positive.Device = DeviceId;
            positive.Axis = (int)_axis;
            positive.AxisValue = 1;
            events.Add(positive);
            return events;
        }

        public DirectionalAction ClearConfig() {
            _positiveKeys = new HashSet<uint>();
            _negativeKeys = new HashSet<uint>();
            _positiveButtons = new HashSet<int>();
            _negativeButtons = new HashSet<int>();
            _axis = JoystickList.InvalidOption;
            return this;
        }

        public DirectionalAction SetDevice(int deviceId) {
            DeviceId = deviceId;
            return this;
        }

        public DirectionalAction SetAllDevices() {
            DeviceId = -1;
            return this;
        }

        public DirectionalAction SetDeadZone(float deadZone) {
            AxisDeadZone = deadZone;
            return this;
        }

        public DirectionalAction AddPositiveKeys(IEnumerable<KeyList> positives) {
            foreach (var i in positives) AddPositiveKey(i);
            return this;
        }

        public DirectionalAction AddNegativeKeys(IEnumerable<KeyList> negatives) {
            foreach (var i in negatives) AddNegativeKey(i);
            return this;
        }

        public DirectionalAction AddPositiveKey(KeyList positive) {
            _positiveKeys.Add((uint)positive);
            return this;
        }

        public DirectionalAction AddNegativeKey(KeyList negative) {
            _negativeKeys.Add((uint)negative);
            return this;
        }

        public DirectionalAction AddPositiveButton(JoystickList positive) {
            _positiveButtons.Add((int)positive);
            return this;
        }

        public DirectionalAction AddPositiveButtons(IEnumerable<JoystickList> positives) {
            foreach (var i in positives) AddPositiveButton(i);
            return this;
        }

        public DirectionalAction AddNegativeButtons(IEnumerable<JoystickList> negatives) {
            foreach (var i in negatives) AddNegativeButton(i);
            return this;
        }

        public DirectionalAction AddNegativeButton(JoystickList negative) {
            _negativeButtons.Add((int)negative);
            return this;
        }

        public DirectionalAction AddLateralCursorKeys() {
            AddPositiveKeys(CursorLateralPositive);
            AddNegativeKeys(CursorLateralNegative);
            return this;
        }

        public DirectionalAction AddVerticalCursorKeys() {
            AddPositiveKeys(CursorVerticalPositive);
            AddNegativeKeys(CursorVerticalNegative);
            return this;
        }

        public DirectionalAction AddLateralDPadButtons() {
            AddPositiveButtons(DPadLateralPositive);
            AddNegativeButtons(DPadLateralNegative);
            return this;
        }

        public DirectionalAction AddVerticalDPadButtons() {
            AddPositiveButtons(DPadVerticalPositive);
            AddNegativeButtons(DPadVerticalNegative);
            return this;
        }

        public DirectionalAction ConfigureAxis(JoystickList axis) {
            _axis = axis;
            return this;
        }
    }
}