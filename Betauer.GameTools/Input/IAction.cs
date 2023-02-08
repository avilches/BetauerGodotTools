using Godot;

namespace Betauer.Input;

public interface IAction {
    public string Name { get; }
    public float Strength { get; }
    public float RawStrength { get; }
    JoyAxis Axis { get; }
    bool IsEvent(InputEvent inputEvent);
    void Enable(bool enabled);
}