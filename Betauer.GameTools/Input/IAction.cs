namespace Betauer.Input;

public interface IAction {
    bool IsJustPressed(bool exact = false);
    bool IsPressed(bool exact = false);
    bool IsReleased(bool exact = false);
    float GetStrength(bool exact = false);
}