using System;
using Godot;

namespace Betauer.UI; 

public partial class ToggleButton : Button {
    private Func<bool> _pressedIf;

    public Func<bool> PressedIf {
        get => _pressedIf;
        set {
            _pressedIf = value;
            Refresh();
        }
    } 

    public void Refresh() {
        ToggleMode = true;
        ButtonPressed = PressedIf?.Invoke() ?? false;
    }
}