using System;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Core;
using Betauer.Nodes;
using Godot;

namespace Betauer.Input;

/// <summary>
/// This class allows you to configure your actions with a specific joypad by id or with the first joypad available. When connected, it will listen if the
/// joypad is disconnected. If it's disconnected, it will try to connect to another joypad. If there is no more joypads, it will call to the
/// OnJoypadDisconnected (so you can show a user message or pause the game). If a new joypad is connected, it will call to the OnJoypadReconnected (so you
/// can unpause the game). If it's already connected, it will ignore any other joypad connections.
/// 
/// You can check at any time is the joypad is connected with the Connected property. If true, the connected joypad is the CurrentJoypad. Pay attention a
/// JoypadId of -1 means connected to any joypad.
/// 
/// Start() will connect to the InputMap JoyConnectionChanged signal and handle OnJoypadReconnected and OnJoypadDisconnected.
/// Stop() will disconnect from the signal. You could use this class for multiple players, in this case, Stop() and Start() could help to avoid
/// unnecessary events.
/// 
/// 
/// </summary>
public class SingleJoypadActionsContainer : InputActionsContainer {
    public bool AllJoypads { get; private set; } = false;
    public int JoypadId { get; private set; } = -1;
    public bool Connected { get; private set; } = false;
    public event Action? OnJoypadReconnected;
    public event Action? OnJoypadDisconnected;

    /// <summary>
    /// If true, it means it's listening for joypad connection and disconnection events in the Godot.InputMap.JoyConnectionChanged signal and dispatching
    /// the OnJoypadReconnected and OnJoypadDisconnected events.
    /// </summary>
    public bool Running { get; private set; } = false;

    public void Start() {
        if (Running) throw new Exception($"Cant' start {GetType().GetTypeName()}: instance is already running.");
        Godot.Input.Singleton.JoyConnectionChanged += OnJoyConnectionChanged;
        Running = true;
    }

    public void Stop() {
        EnsureRunning();
        Godot.Input.Singleton.JoyConnectionChanged -= OnJoyConnectionChanged;
        Running = false;
    }

    private void EnsureRunning() {
        if (!Running) throw new Exception($"Cant' stop {GetType().GetTypeName()}: instance is not running.");
    }

    /// <summary>
    /// Try to connect to the first joypad available. If there is no joypad to connect to, it will return false and the previous state will remains without
    /// any change (Connected, JoypadId and AllJoypads).
    /// If there is joypad available , it will return true and it will remap al the actions to this joypad, it will set Connected to true and JoypadId to
    /// the given joypadId. It also set AllJoypads to false.
    /// </summary>
    /// <returns></returns>
    public bool SetFirstConnectedJoypad() {
        var joypads = Godot.Input.GetConnectedJoypads();
        if (joypads.Count == 0) {
            return false;
        }
        AllJoypads = false;
        var joypadId = joypads[0];
        JoypadId = joypadId;
        Connected = true;
        UpdateInputActionsJoypad(joypadId);
        return true;
    }

    /// <summary>
    /// Try to connect to the joypad with the given id. If it's not connected, it will return false. If it's connected, it will return true and it will
    /// remap al the actions to the new joypad, it will set Connected to true and JoypadId to the given joypadId. It also set AllJoypads to false.
    /// </summary>
    /// <param name="joypadId"></param>
    /// <returns></returns>
    public bool SetJoypad(int joypadId) {
        var joypads = Godot.Input.GetConnectedJoypads().ToHashSet();
        if (!joypads.Contains(joypadId)) {
            return false;
        }
        AllJoypads = false;
        JoypadId = joypadId;
        Connected = true;
        UpdateInputActionsJoypad(joypadId);
        return true;
    }

    /// <summary>
    /// It connects to all the joypads available, no matter the number of joypads connected. It will set AllJoypads to true, Connected to true and JoypadId to -1
    /// </summary>
    public void SetAllJoypads() {
        if (AllJoypads) return;
        AllJoypads = true;
        JoypadId = -1;
        Connected = Godot.Input.GetConnectedJoypads().Count > 0;
        UpdateInputActionsJoypad(-1);
    }

    public int[] GetJoypadIdsConnected() {
        return Godot.Input.GetConnectedJoypads().ToArray();
    }

    private void OnJoyConnectionChanged(long device, bool connected) {
        if (AllJoypads) {
            if (!connected) {
                Connected = Godot.Input.GetConnectedJoypads().Count > 0;
            }
        } else {
            var deviceId = (int)device;
            if (!Connected && connected) {
                // New joypad connected when there wasn't any joypad! Let's use it!
                JoypadId = deviceId;
                Connected = true;
                UpdateInputActionsJoypad(deviceId);
                OnJoypadReconnected?.Invoke();
            } else if (!connected && deviceId == JoypadId) {
                // The current joypad is disconnected, try to connect other
                if (SetFirstConnectedJoypad()) {
                    OnJoypadReconnected?.Invoke();
                } else {
                    Connected = false;
                    OnJoypadDisconnected?.Invoke();
                }
            }
        }
    }

    // TODO: mover esto a
    public async Task<int> WaitForAnyJoypadButton(float timeoutSeconds = 0) {
        InputEventJoypadButton? inputEvent =
            await NodeManager.MainInstance.AwaitInput<InputEventJoypadButton>(
                joypadButton => joypadButton.Pressed,
                timeoutSeconds);
        return inputEvent?.Device ?? -1;
    }

    public async Task<int> WaitForJoypadButton(JoyButton button, float timeoutSeconds = 0) {
        InputEventJoypadButton? joypadButton =
            await NodeManager.MainInstance.AwaitInput<InputEventJoypadButton>(
                joypadButton => joypadButton.Pressed && button == joypadButton.ButtonIndex,
                timeoutSeconds);
        return joypadButton?.Device ?? -1;
    }

    public async Task<int> WaitForJoypadButtons(JoyButton[] buttons, float timeoutSeconds = 0) {
        InputEventJoypadButton? joypadButton =
            await NodeManager.MainInstance.AwaitInput<InputEventJoypadButton>(
                joypadButton => joypadButton.Pressed && buttons.Contains(joypadButton.ButtonIndex),
                timeoutSeconds);
        return joypadButton?.Device ?? -1;
    }

    private void UpdateInputActionsJoypad(int joypadId) {
        InputActions.ForEach(action => action.JoypadId = joypadId);
    }
}