using System.Collections.Generic;
using Betauer.Core.Time;
using Godot;

namespace Betauer.Input.Handler;

internal class GodotInputHandler : IHandler {
    public const float MaxPressedTime = 31536000f; // 1 year! 

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
    public float PressedTime => _stopwatchPressed != null ? (float)_stopwatchPressed.Elapsed : MaxPressedTime;
    public float ReleasedTime => _stopwatchReleased != null ? (float)_stopwatchReleased.Elapsed : MaxPressedTime;
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

    public void UpdateJustTimers() {
        if (JustPressed) {
            _stopwatchPressed!.Restart();
        } else if (JustReleased) {
            _stopwatchReleased!.Restart();
        }
    }

    public void Refresh(InputAction inputAction) {
        var stringName = (StringName)inputAction.Name;
        if (InputMap.HasAction(stringName)) {
            InputMap.EraseAction(stringName);
        }
        if (inputAction.Enabled) {
            InputAction.Logger.Info("Adding action: {0} Joypad:{1}", inputAction.Name, inputAction.JoypadId);
            InputMap.AddAction(stringName, inputAction.DeadZone);
            CreateInputEvents(inputAction).ForEach(e => InputMap.ActionAddEvent(stringName, e));
        }
    }

    private List<InputEvent> CreateInputEvents(InputAction inputAction) {
        List<InputEvent> events = new List<InputEvent>(inputAction.Keys.Count + inputAction.Buttons.Count + 1);
        foreach (var key in inputAction.Keys) {
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
            var e = new InputEventJoypadButton();
            e.Device = inputAction.JoypadId;
            e.ButtonIndex = button;
            events.Add(e);
        }

        if (inputAction.Axis != JoyAxis.Invalid && inputAction.AxisSign != 0) {
            var e = new InputEventJoypadMotion();
            e.Device = inputAction.JoypadId;
            e.Axis = inputAction.Axis;
            e.AxisValue = inputAction.AxisSign;
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
}