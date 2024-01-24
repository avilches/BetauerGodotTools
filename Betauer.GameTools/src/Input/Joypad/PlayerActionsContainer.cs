using System;
using System.Linq;
using Godot;

namespace Betauer.Input.Joypad;

public class PlayerActionsContainer {

    public event Action? OnJoypadReconnected;
    public event Action? OnJoypadDisconnected;

    public bool Connected { get; private set; } = false;
    public int PlayerId { get; private set; } = -1;
    public int JoypadId { get; private set; } = -1;
    public bool Keyboard { get; private set; } = false;

    public InputActionsContainer InputActionsContainer { get;  } = new();
    private InputActionsContainer _inputActionsContainerSource;

    internal void Start(int playerId, int joypadId, bool keyboard, InputActionsContainer inputActionsContainerSource) {
        PlayerId = playerId;
        JoypadId = joypadId;
        Keyboard = keyboard;
        Connected = Godot.Input.GetConnectedJoypads().ToArray().Contains(JoypadId);
        _inputActionsContainerSource = inputActionsContainerSource;
        InputActionsContainer.Clear();
        InputActionsContainer.AddActionsFromProperties(this);
        SyncActions();
        InputActionsContainer.EnableAll();
    }

    internal void Stop() {
        PlayerId = -1;
        JoypadId = -1;
        Keyboard = false;
        Connected = false;
        InputActionsContainer.Clear();
    }

    /// <summary>
    /// This method is used when the player redefine some actions in the PlayerActionsContainer so the changes are reflected in the InputActionsContainer.
    /// </summary>
    /// <exception cref="Exception"></exception>
    internal void SyncActions() {
        var suffix = PlayerId.ToString();
        InputActionsContainer.InputActions.ForEach(action => {
            // The first time, the name is just "Jump", but the second time it could be "Jump/P1" or "Jump/P4"
            var inputActionName = action.Name.Split("/")[0]; 
            var defaultAction = _inputActionsContainerSource.GetInputAction(inputActionName);
            if (defaultAction == null) {
                throw new Exception($"Action {inputActionName} not found in default player actions container");
            }
            // Disable it first to avoid calling to RefreshGodotInputMap() in Update(), ChangeName() and setting the JoypadId
            var wasEnabled = action.Enabled;
            action.Disable();
            action.Update(u => {
                u.ClearAll();
                if (Keyboard) {
                    u.CopyKeyboardAndMouse(defaultAction);
                }
                if (JoypadId >= 0) {
                    u.CopyJoypad(defaultAction);
                }
            });
            action.ChangeName($"{inputActionName}/P{suffix}");
            action.JoypadId = JoypadId;
            if (wasEnabled) action.Enable();
        });
    }

    internal void ChangePlayerId(int newPlayerId) {
        if (newPlayerId == PlayerId) return;
        PlayerId = newPlayerId;
        var suffix = PlayerId.ToString();
        InputActionsContainer.InputActions.ForEach(action => {
            // The first time, the name is just "Jump", but the second time it could be "Jump/P1" or "Jump/P4"
            var inputActionName = action.Name.Split("/")[0]; 
            action.ChangeName($"{inputActionName}/P{suffix}");
        });
    }

    public void ChangeJoypad(int newJoypadId) {
        if (newJoypadId == JoypadId) return;
        JoypadId = newJoypadId;
        InputActionsContainer.InputActions.ForEach(action => action.JoypadId = JoypadId);
    }

    public void ChangeKeyboard(bool newKeyboard) {
        if (newKeyboard == Keyboard) return;
        Keyboard = newKeyboard;
        SyncActions();
    }

    internal void JoyConnectionChanged(bool connected) {
        if (Connected == connected) return;
        Connected = connected;
        if (Connected) OnJoypadReconnected?.Invoke();
        else OnJoypadDisconnected?.Invoke();
    }

    /// <summary>
    /// <para>Returns <c>true</c> if you are pressing the joypad button (see <see cref="T:Godot.JoyButton" />).</para>
    /// </summary>
    public bool IsJoyButtonPressed(JoyButton button) => Godot.Input.IsJoyButtonPressed(JoypadId, button);

    /// <summary>
    /// <para>Returns <c>true</c> if the system knows the specified device. This means that it sets all button and axis indices. Unknown joypads are not expected to match these constants, but you can still retrieve events from them.</para>
    /// </summary>
    public bool IsJoyKnown() => Godot.Input.IsJoyKnown(JoypadId);

    /// <summary>
    /// <para>Returns the current value of the joypad axis at given index (see <see cref="T:Godot.JoyAxis" />).</para>
    /// </summary>
    public float GetJoyAxis(JoyAxis axis) => Godot.Input.GetJoyAxis(JoypadId, axis);

    /// <summary>
    /// <para>Returns the name of the joypad at the specified device index, e.g. <c>PS4 Controller</c>. Godot uses the <a href="https://github.com/gabomdq/SDL_GameControllerDB">SDL2 game controller database</a> to determine gamepad names.</para>
    /// </summary>
    public string GetJoyName() => Godot.Input.GetJoyName(JoypadId);

    /// <summary>
    /// <para>Returns a SDL2-compatible device GUID on platforms that use gamepad remapping, e.g. <c>030000004c050000c405000000010000</c>. Returns <c>"Default Gamepad"</c> otherwise. Godot uses the <a href="https://github.com/gabomdq/SDL_GameControllerDB">SDL2 game controller database</a> to determine gamepad names and mappings based on this GUID.</para>
    /// </summary>
    public string GetJoyGuid() => Godot.Input.GetJoyGuid(JoypadId);
    
    /// <summary>
    /// <para>Returns the strength of the joypad vibration: x is the strength of the weak motor, and y is the strength of the strong motor.</para>
    /// </summary>
    public Vector2 GetJoyVibrationStrength() => Godot.Input.GetJoyVibrationStrength(JoypadId);

    /// <summary>
    /// <para>Returns the duration of the current vibration effect in seconds.</para>
    /// </summary>
    public float GetJoyVibrationDuration() => Godot.Input.GetJoyVibrationDuration(JoypadId);

    /// <summary>
    /// <para>Starts to vibrate the joypad. Joypads usually come with two rumble motors, a strong and a weak one. <paramref name="weakMagnitude" /> is the strength of the weak motor (between 0 and 1) and <paramref name="strongMagnitude" /> is the strength of the strong motor (between 0 and 1). <paramref name="duration" /> is the duration of the effect in seconds (a duration of 0 will try to play the vibration indefinitely). The vibration can be stopped early by calling <see cref="M:Godot.Input.StopJoyVibration(System.Int32)" />.</para>
    /// <para><b>Note:</b> Not every hardware is compatible with long effect durations; it is recommended to restart an effect if it has to be played for more than a few seconds.</para>
    /// </summary>
    public void StartJoyVibration(float weakMagnitude, float strongMagnitude, float duration = 0.0f) => Godot.Input.StartJoyVibration(JoypadId, weakMagnitude, strongMagnitude, duration);

    /// <summary>
    /// <para>Stops the vibration of the joypad started with <see cref="M:Godot.Input.StartJoyVibration(System.Int32,System.Single,System.Single,System.Single)" />.</para>
    /// </summary>
    public void StopJoyVibration() => Godot.Input.StopJoyVibration(JoypadId);

    public override string ToString() {
        return $"P{PlayerId}:{JoypadId}{(Connected ? "" : "(disconnected)")}";
    }
}