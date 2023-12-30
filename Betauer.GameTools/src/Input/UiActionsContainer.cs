using System;
using System.Linq;
using Betauer.Core;

namespace Betauer.Input;

public partial class UiActionsContainer : InputActionsContainer {

    public int BackupJoypad { get; private set; } = 0;
    public int CurrentJoyPad { get; private set; } = 0;
    
    public event Action<int>? OnNewUiJoypad;

    public UiActionsContainer() {
        Godot.Input.Singleton.JoyConnectionChanged += JoyConnectionChanged;
    }

    private void JoyConnectionChanged(long deviceId, bool connected) {
        if (connected && deviceId == BackupJoypad) {
            // The original controller has ben re-connected, use it again
            SetJoypad(BackupJoypad);
        } else if (!connected && deviceId == CurrentJoyPad) {
            // The current joy pad is disconnected, try to connect other as temporal
            var pads = Godot.Input.GetConnectedJoypads();
            if (pads.Count > 0) {
                SetTemporalJoypad(pads[0]);
            }
        }
    }

    public void SetFirstJoypadConnected() {
        var pads = Godot.Input.GetConnectedJoypads();
        var joypad = pads.Count > 0 ? pads[0] : 0;
        SetJoypad(joypad);
    }

    public void SetJoypad(int joypadId) {
        CurrentJoyPad = joypadId;
        BackupJoypad = joypadId;
        SetJoypadAllInputActions(joypadId);
    }

    public void SetTemporalJoypad(int joypadId) {
        CurrentJoyPad = joypadId;
        SetJoypadAllInputActions(joypadId);
    }

    private void SetJoypadAllInputActions(int joypadId) {
        InputActionList.OfType<InputAction>().ForEach(inputAction => {
            inputAction.Update(updater => {
                updater.SetJoypadId(joypadId);
            });
        });
        OnNewUiJoypad?.Invoke(joypadId);
    }
}