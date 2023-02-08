namespace Betauer.Input;

internal interface IHandler {
    internal bool Pressed { get; }
    internal float Strength { get; }
    internal float RawStrength { get; }
    internal float PressedTime { get; }
    internal float ReleasedTime { get; }
    internal bool JustPressed { get; }
    internal bool JustReleased { get; }
    internal void SimulatePress(float strength);
}