using System;
using Godot;

namespace Betauer.Input.Joypad;

public class PlayerActionsContainer : InputActionsContainer {
    private int _playerId = -1;
    private int _joypadId = -1;
    private bool _started = false;

    public event Action? OnJoypadDisconnect;
    public event Action? OnJoypadConnect;
    public event Action? OnJoypadIdChanged;

    public bool Connected { get; internal set; } = false;

    public int PlayerId {
        get => _playerId;
        private set {
            if (_playerId == value) return;
            _playerId = value;
            _PlayerIdChanged();
        }
    }

    public int JoypadId {
        get => _joypadId;
        private set {
            if (_joypadId == value) return;
            _joypadId = value;
            _JoypadIdChanged();
        }
    }

    public void SetAllJoypads() => JoypadId = -1;

    public void Start(int playerId, int joypadId) {
        DisableAll();
        PlayerId = playerId;
        JoypadId = joypadId;
        EnableAll();
        if (!_started) {
            _started = true;
            Godot.Input.Singleton.JoyConnectionChanged += JoyConnectionChanged;
        }
    }

    public void Stop() {
        DisableAll();
        if (_started) {
            _started = false;
            Godot.Input.Singleton.JoyConnectionChanged -= JoyConnectionChanged;
        }
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

    private void JoyConnectionChanged(long device, bool connected) {
        if (device != JoypadId) return;
        if (Connected == connected) return;
        Connected = connected;
        _ConnectedChanged();
    }

    private void _ConnectedChanged() {
        if (Connected) OnJoypadConnect?.Invoke();
        else OnJoypadDisconnect?.Invoke();
    }

    private void _PlayerIdChanged() {
        var suffix = PlayerId.ToString();
        InputActions.ForEach(action => { action.ChangeName($"{action.Name}/{suffix}"); });
    }

    private void _JoypadIdChanged() {
        InputActions.ForEach(action => { action.Update(u => u.SetJoypadId(JoypadId)); });
        Connected = Godot.Input.GetConnectedJoypads().Contains(JoypadId);
        OnJoypadIdChanged?.Invoke();
    }

    public override string ToString() {
        return $"P{PlayerId}:{JoypadId}{(Connected ? "" : "(disconnected)")}";
    }
}