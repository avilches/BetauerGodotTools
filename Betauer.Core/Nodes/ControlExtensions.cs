using Godot;

namespace Betauer.Nodes {
    public static partial class ControlExtensions {
        public static T SetFontColor<T>(this T label, Color color) where T : Control {
            label.AddThemeColorOverride("font_color", color);
            return label;
        }
    }
}
