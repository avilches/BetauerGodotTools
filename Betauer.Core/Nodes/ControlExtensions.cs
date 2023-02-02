using Godot;

namespace Betauer.Core.Nodes; 

public static partial class ControlExtensions {
    public static T SetFontColor<T>(this T control, Color color) where T : Control {
        control.AddThemeColorOverride("font_color", color);
        return control;
    }
    
    public static T RemoveFontColor<T>(this T control) where T : Control {
        control.RemoveThemeColorOverride("font_color");
        return control;
    }
}