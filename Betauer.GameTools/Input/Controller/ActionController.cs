namespace Betauer.Input.Controller; 

public class ActionController : IAction {
    private bool _hold = false;
    private bool _pressed = false;
    private bool _justPressed = false;
    private bool _released = false;
    private float _strength = 0f;

    public bool IsJustPressed(bool exact = false) => _justPressed;
    public bool IsPressed(bool exact = false) => _pressed;
    public bool IsReleased(bool exact = false) => _released;
    public float GetStrength(bool exact = false) => _strength;
    
    public void PressAndHold(float strength = 1f) {
        _hold = true;
        _pressed = true;
        _justPressed = true;
        _released = false;
        _strength = strength;
    }

    public void Hold(float strength = 1f) {
        _hold = true;
        _pressed = true;
        _justPressed = false;
        _released = false;
        _strength = strength;
    }

    public void QuickPress(float strength = 1f) {
        _hold = false;
        _pressed = true;
        _justPressed = true;
        _released = false;
        _strength = strength;
    }

    public void Release() {
        if (_pressed && _hold) {
            _hold = false;
            _pressed = false;
            _justPressed = false;
            _released = true;
            _strength = 0f;
        }
    }

    public void EndFrame() {
        if (_hold) {
            _justPressed = false;
        } else {
            _hold = false;
            _pressed = false;
            _justPressed = false;
            _released = false;
            _strength = 0f;
        }
    }
}