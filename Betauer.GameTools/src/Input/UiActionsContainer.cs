using System;

namespace Betauer.Input;

public class UiActionsContainer : InputActionsContainer {

    public int OriginalJoypad { get; private set; } = 0;
    public int CurrentJoypad { get; private set; } = 0;
    
    public event Action<int>? OnNewUiJoypad;
    
    private bool _initialized = false;

    public void ConfigureOnlyOnePlayerUI() {
        if (!_initialized) {
            _initialized = true;
            Godot.Input.Singleton.JoyConnectionChanged += JoyConnectionChanged;
        }
    }

    private void JoyConnectionChanged(long deviceId, bool connected) {
        if (connected && deviceId == OriginalJoypad) {
            // The original controller has ben re-connected, use it again
            SetJoypad(OriginalJoypad);
        } else if (!connected && deviceId == CurrentJoypad) {
            // The current joy pad is disconnected, try to connect other as temporal
            var pads = Godot.Input.GetConnectedJoypads();
            if (pads.Count > 0) {
                var joypadId = pads[0];
                CurrentJoypad = joypadId;
                SetJoypadAllInputActions(joypadId);
            }
        }
    }

    public void SetFirstConnectedJoypad() {
        var pads = Godot.Input.GetConnectedJoypads();
        var joypad = pads.Count > 0 ? pads[0] : 0;
        SetJoypad(joypad);
    }

    public void SetAllJoypads() {
        SetJoypad(-1);
    }

    public void SetJoypad(int joypadId) {
        CurrentJoypad = joypadId;
        OriginalJoypad = joypadId;
        SetJoypadAllInputActions(joypadId);
        OnNewUiJoypad?.Invoke(joypadId);
    }

    private void SetJoypadAllInputActions(int joypadId) {
        InputActions.ForEach(action => action.Update(u => u.SetJoypadId(joypadId))); 
    }
}