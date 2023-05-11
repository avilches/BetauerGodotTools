using System;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Signal;

namespace Betauer.Input;

public partial class UiActionsContainer : InputActionsContainer {

    public int BackupJoypad { get; private set; } = 0;
    public int CurrentJoyPad { get; private set; } = 0;
    public event Action<int> OnNewUiJoypad; 

    private void _Change(int joypadId) {
        InputActionList.OfType<InputAction>().ForEach(inputAction => {
            inputAction.Update(updater => {
                updater.SetJoypadId(joypadId);
            });
        });
        OnNewUiJoypad?.Invoke(joypadId);
    }

    public void SetTemporalJoypad(int joypadId) {
        CurrentJoyPad = joypadId;
        _Change(joypadId);
    }

    public void SetJoypad(int joypadId) {
        CurrentJoyPad = joypadId;
        BackupJoypad = joypadId;
        _Change(joypadId);
    }

    public void Start(int joypad = -1) {
        if (joypad < 0) {
            var pads = Godot.Input.GetConnectedJoypads();
            joypad = pads.Count > 0 ? pads[0] : 0;
        }
        SetJoypad(joypad);
        
        // TODO: Godot 4 lacks a way to disconnect callable lambdas
        SignalExtensions.OnInputJoyConnectionChanged((device, connected) => {
            if (connected) { // Connect
                if (device == BackupJoypad) {
                    SetJoypad(BackupJoypad);
                }
            } else { // Disconnect
                if (device == CurrentJoyPad) {
                    var pads = Godot.Input.GetConnectedJoypads();
                    if (pads.Count > 0) {
                        SetTemporalJoypad(pads[0]);
                    }
                }
            }
        });
    }
}