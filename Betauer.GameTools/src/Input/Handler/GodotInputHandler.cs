using System.Collections.Generic;
using Betauer.Core.Time;
using Betauer.Nodes;
using Godot;

namespace Betauer.Input.Handler;

internal class GodotInputHandler : IHandler {

    internal readonly InputAction InputAction;
    private StringName? _stringName;
    private readonly GodotStopwatch? _stopwatchPressed;
    private readonly GodotStopwatch? _stopwatchReleased;

    public bool HasJustTimers => _stopwatchPressed != null;

    // Don't cache the string name in the constructor.
    // Delay the StringName creation until it's actually needed. Reason: the InputAction.Name could be empty and changed later through [InputAction] attribute.
    private StringName ActionStringName => _stringName ??= new StringName(InputAction.Name);

    public GodotInputHandler(InputAction inputAction, bool loadWasPressed) {
        if (loadWasPressed) {
            _stopwatchPressed = new GodotStopwatch().Start();
            _stopwatchReleased = new GodotStopwatch().Start();
        }
        InputAction = inputAction;
    }

    public bool Pressed => Godot.Input.IsActionPressed(ActionStringName);
    public float Strength => Godot.Input.GetActionStrength(ActionStringName);
    public float RawStrength => Godot.Input.GetActionRawStrength(ActionStringName);
    public float PressedTime => _stopwatchPressed != null ? (float)_stopwatchPressed.Elapsed : 0;
    public float ReleasedTime => _stopwatchReleased != null ? (float)_stopwatchReleased.Elapsed : 0;
    public bool JustPressed => Godot.Input.IsActionJustPressed(ActionStringName);
    public bool JustReleased => Godot.Input.IsActionJustReleased(ActionStringName);

    public void SimulatePress(float strength) {
        if (strength >= 0.001f) {
            Godot.Input.ActionPress(ActionStringName, strength);
        } else {
            Godot.Input.ActionRelease(ActionStringName);
        }
    }

    public void SimulateRelease() {
        Godot.Input.ActionRelease(ActionStringName);
    }

    public void ClearJustStates() {
    }

    public void Refresh() {
        var stringName = (StringName)InputAction.Name;
        if (InputMap.HasAction(stringName)) {
            InputMap.EraseAction(stringName);
        }
        if (InputAction.Enabled) {
            InputMap.AddAction(stringName, InputAction.DeadZone);
            CreateInputEvents(InputAction).ForEach(e => InputMap.ActionAddEvent(stringName, e));
            InputAction.Logger.Info("Adding action: {0} J:{1} {2}", InputAction.Name, InputAction.JoypadId, InputAction.Export());
        }

        if (HasJustTimers) {
            NodeManager.MainInstance.OnInput -= UpdateJustTimers;
            if (InputAction.Enabled) NodeManager.MainInstance.OnInput += UpdateJustTimers;
        }
    }

    private List<InputEvent> CreateInputEvents(InputAction inputAction) {
        List<InputEvent> events = new List<InputEvent>(inputAction.Keys.Count + inputAction.Buttons.Count + 1);
        foreach (var key in inputAction.Keys) {
            if (key is Key.Unknown or Key.None) continue;
            var e = new InputEventKey();
            // TODO: if (KeyboardDeviceId >= 0) e.Device = KeyboardDeviceId;
            e.Keycode = key;
            AddModifiers(e);
            events.Add(e);
        }
        if (inputAction.MouseButton != MouseButton.None) {
            var e = new InputEventMouseButton();
            // TODO: if (MouseDeviceId >= 0) e.Device = MouseDeviceId;
            e.ButtonIndex = inputAction.MouseButton;
            AddModifiers(e);
            events.Add(e);
        }
        foreach (var button in inputAction.Buttons) {
            if (button == JoyButton.Invalid) continue;
            var e = new InputEventJoypadButton();
            e.Device = inputAction.JoypadId;
            e.ButtonIndex = button;
            events.Add(e);
        }
        if (inputAction.Axis != JoyAxis.Invalid) {
            var e = new InputEventJoypadMotion();
            e.Device = inputAction.JoypadId;
            e.Axis = inputAction.Axis;
            e.AxisValue = (int)inputAction.AxisSign;
            events.Add(e);
        }
        return events;

        void AddModifiers(InputEventWithModifiers e) {
            e.ShiftPressed = inputAction.Shift;
            e.AltPressed = inputAction.Alt;
            if (inputAction.CommandOrCtrlAutoremap) {
                e.CommandOrControlAutoremap = true;
            } else {
                e.CtrlPressed = inputAction.Ctrl;
                e.MetaPressed = inputAction.Meta;
            }
        }
    }

    private void UpdateJustTimers(InputEvent e) {
        if (InputAction.IsJustPressed) {
            _stopwatchPressed!.Restart();
        } else if (InputAction.IsJustReleased) {
            _stopwatchReleased!.Restart();
        }
    }
}