using System;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Signal;

namespace Betauer.Input;

public partial class UiActionsContainer : InputActionsContainer {

    public int FirstJoyPad { get; private set; } = 0;
    public int CurrentJoyPad { get; private set; } = 0;
    public event Action<int> OnNewUiJoypad; 

    private Action _joyConnectionChangedSignal;

    public void SetJoypadDeviceId(int joypadDeviceId) {
        InputActionList.OfType<InputAction>().ForEach(inputAction => {
            inputAction.Update(updater => {
                updater.SetJoypadDevice(joypadDeviceId);
            });
        });
        OnNewUiJoypad?.Invoke(CurrentJoyPad);
    }

    public void Start() {
        var pads = Godot.Input.GetConnectedJoypads();
        FirstJoyPad = pads.Count > 0 ? pads[0] : 0; 
        CurrentJoyPad = FirstJoyPad;
        SetJoypadDeviceId(FirstJoyPad);
        
        _joyConnectionChangedSignal = SignalExtensions.OnInputJoyConnectionChanged((device, connected) => {
            if (connected) { // Connect
                if (device == FirstJoyPad && device != CurrentJoyPad) {
                    CurrentJoyPad = FirstJoyPad;
                    SetJoypadDeviceId(CurrentJoyPad);
                }
            } else { // Disconnect
                if (device == CurrentJoyPad) {
                    var pads = Godot.Input.GetConnectedJoypads();
                    if (pads.Count > 0) {
                        CurrentJoyPad = pads[0];
                        SetJoypadDeviceId(CurrentJoyPad);
                    }
                }
            }
        });
    }

    public void Destroy() {
        _joyConnectionChangedSignal?.Invoke();
        _joyConnectionChangedSignal = null;
    }
}