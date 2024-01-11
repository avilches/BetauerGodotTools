using System;
using System.Linq;
using Betauer.Core;
using Betauer.DI;

namespace Betauer.Input;

public abstract class UiActionsContainer : InputActionsContainer, IInjectable {

    public int BackupJoypad { get; private set; } = 0;
    public int CurrentJoyPad { get; private set; } = 0;
    
    public event Action<int>? OnNewUiJoypad;

    public UiActionsContainer() {
        Godot.Input.Singleton.JoyConnectionChanged += JoyConnectionChanged;
    }

    public void PostInject() {
        // TODO: What if there are more than one InputActionsContainer? Only the last one will have the command linked
        // DebugOverlayManager?.DebugConsole.AddInputEventCommand(this);
        // DebugOverlayManager?.DebugConsole.AddInputMapCommand(this);
        LoadFromInstance(this);
        EnableAll();
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

    public void SetAllJoypads() {
        SetJoypad(-1);
    }

    public void SetJoypad(int joypadId) {
        CurrentJoyPad = joypadId;
        BackupJoypad = joypadId;
        SetJoypadAllInputActions(joypadId);
        OnNewUiJoypad?.Invoke(joypadId);
    }

    public void SetTemporalJoypad(int joypadId) {
        CurrentJoyPad = joypadId;
        SetJoypadAllInputActions(joypadId);
    }

    private void SetJoypadAllInputActions(int joypadId) {
        InputActions.ForEach(inputAction => {
            inputAction.Update(updater => {
                updater.SetJoypadId(joypadId);
            });
        });
    }
}