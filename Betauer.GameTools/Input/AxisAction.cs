using Godot;

namespace Betauer.Input;

public class AxisAction {
    public readonly string NegativeName;
    public readonly string PositiveName;

    public float Strength => Godot.Input.GetAxis(NegativeName, PositiveName);

    internal AxisAction(string negativeName, string positiveName) {
        NegativeName = negativeName;
        PositiveName = positiveName;
    }

    public bool IsRightEventPressed(InputEvent e, bool echo = false) {
        return e.IsActionPressed(PositiveName, echo);
    }

    public bool IsLeftEventPressed(InputEvent e, bool echo = false) {
        return e.IsActionPressed(NegativeName, echo);
    }
        
    public bool IsRightEventReleased(InputEvent e, bool echo = false) {
        return e.IsActionReleased(PositiveName, echo);
    }

    public bool IsLeftEventReleased(InputEvent e, bool echo = false) {
        return e.IsActionReleased(NegativeName, echo);
    }

    public bool IsDownEventPressed(InputEvent e, bool echo = false) {
        return e.IsActionPressed(PositiveName, echo);
    }

    public bool IsUpEventPressed(InputEvent e, bool echo = false) {
        return e.IsActionPressed(NegativeName, echo);
    }
        
    public bool IsDownEventReleased(InputEvent e, bool echo = false) {
        return e.IsActionReleased(PositiveName, echo);
    }

    public bool IsUpEventReleased(InputEvent e, bool echo = false) {
        return e.IsActionReleased(NegativeName, echo);
    }
}