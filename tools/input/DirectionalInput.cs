using System;
using System.Collections.Generic;
using Godot;

public class DirectionInput : IActionUpdate {
    public static ISet<int> Empty = new HashSet<int>();
    public static ISet<int> CursorPositive = new HashSet<int>{ (int)KeyList.Right };
    public static ISet<int> CursorNegative = new HashSet<int>{ (int)KeyList.Left };

    private ISet<int> _positives = CursorPositive;
    private ISet<int> _negatives = CursorNegative;
    private JoystickList _axis = JoystickList.Axis0;
    private readonly PlayerController _playerController;

    private float _inputPositive = 0f;
    private float _inputNegative = 0f;
    private float _input = 0f;
    private float _axisValue = 0f;

    public string Name;
    public float AxisDeadZone = 0.5f;
    public float Strength => _playerController.IsUsingKeyboard ? _input : _axisValue;

    public DirectionInput(string name, PlayerController playerController) {
        Name = name;
        _playerController = playerController;
    }

    public override bool Update(EventWrapper w) {
        if (!Enabled) return false;
        if (LateralAxisMovement(w)) {
            _playerController.IsUsingKeyboard = false;
            return true;
        }

        if (LateralKeyMovement(w)) {
            _playerController.IsUsingKeyboard = true;
            _input = _inputPositive - _inputNegative;
            return true;
        }

        return false;
    }

    public override void ClearJustState() {
    }

    private bool LateralKeyMovement(EventWrapper w) {
        if (w.IsKey(_positives)) {
            _inputPositive = w.IsPressed() ? w.GetStrength() : 0;
            return true;
        } else if (w.IsKey(_negatives)) {
            _inputNegative = w.IsPressed() ? w.GetStrength() : 0;
            return true;
        }

        return false;
    }

    private bool LateralAxisMovement(EventWrapper w) {
        if (w.IsAxis((int) _axis)) {
            _axisValue = w.GetStrength(AxisDeadZone);
            return true;
        }

        return false;
    }

    public DirectionInput ConfigureDefaults() {
        return Configure(CursorPositive, CursorNegative, JoystickList.Axis0, 0.5F);
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