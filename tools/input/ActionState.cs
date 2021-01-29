using System.Collections.Generic;
using Godot;

public class ActionState: IActionUpdate {
    private ISet<int> _buttons;
    private ISet<int> _keys;

    public bool Pressed { get; private set; } = false;
    public bool JustPressed { get; private set; } = false;
    public bool JustReleased { get; private set; } = false;

    private readonly PlayerController _playerController;
    public string Name;

    public ActionState(string name, PlayerController playerController) {
        Name = name;
        _playerController = playerController;
    }

    public override bool Update(EventWrapper w) {
        if (!Enabled) return false;
        if (w.IsKey(_keys) || w.IsButton(_buttons)) {
            _playerController.IsUsingKeyboard = true;
            if (w.IsPressed()) {
                JustPressed = !Pressed;
                Pressed = true;
                JustReleased = false;
            } else {
                JustReleased = Pressed;
                Pressed = JustPressed = false;
            }
            return true;
        }

        ClearJustState();
        return false;
    }

    public override void ClearJustState() {
        JustPressed = JustReleased = false;
    }

    public ActionState Configure(KeyList key, JoystickList button) {
        return Configure(new HashSet<int>{(int)key}, new HashSet<int>{(int)button});
    }

    public ActionState Configure(ISet<int> keys, ISet<int> buttons) {
        _keys = keys;
        _buttons = buttons;
        return this;
    }

}