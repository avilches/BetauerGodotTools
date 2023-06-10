using Godot;

namespace Veronenger.UI.Consoles;

public interface IControllerSpriteConfig {
    Texture2D Texture2D { get; }
    ConsoleButtonView? Get(JoyButton button);
    ConsoleButtonView? Get(JoyAxis axis);
}