using System;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Signal;

namespace Betauer.Input;

public partial class UiActionsContainer : InputActionsContainer {

    public int BackupJoypad { get; private set; } = 0;
    public int CurrentJoyPad { get; private set; } = 0;
    public event Action<int> OnNewUiJoypad; 

    private Action? _disconnectGodotSignal;

    private void _Change(int joypadDeviceId) {
        InputActionList.OfType<InputAction>().ForEach(inputAction => {
            inputAction.Update(updater => {
                updater.SetJoypadDevice(joypadDeviceId);
            });
        });
        OnNewUiJoypad?.Invoke(joypadDeviceId);
    }

    public void SetTemporalJoypad(int joypadDeviceId) {
        CurrentJoyPad = joypadDeviceId;
        _Change(joypadDeviceId);
    }

    public void SetJoypad(int joypadDeviceId) {
        CurrentJoyPad = joypadDeviceId;
        BackupJoypad = joypadDeviceId;
        _Change(joypadDeviceId);
    }

    public void Start(int joypad = -1) {
        if (joypad < 0) {
            var pads = Godot.Input.GetConnectedJoypads();
            joypad = pads.Count > 0 ? pads[0] : 0;
        }
        SetJoypad(joypad);
        
        _disconnectGodotSignal ??= SignalExtensions.OnInputJoyConnectionChanged((device, connected) => {
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

    public void Destroy() {
        _disconnectGodotSignal?.Invoke();
        _disconnectGodotSignal = null;
    }
}