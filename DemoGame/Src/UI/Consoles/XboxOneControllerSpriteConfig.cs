using Betauer.DI;
using Godot;

namespace Veronenger.UI.Consoles;

[Singleton]
public class XboxOneControllerSpriteConfig : HyohnooSpriteConfig {
    [Inject]
    public Texture2D XboxOneButtons { get; set; }

    public override Texture2D Texture2D => XboxOneButtons;
}