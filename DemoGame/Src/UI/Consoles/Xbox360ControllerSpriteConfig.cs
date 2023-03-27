using Betauer.DI;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.UI.Consoles;

[Singleton]
public class Xbox360ControllerSpriteConfig : HyohnooSpriteConfig {
    [Inject]
    public Texture2D Xbox360Buttons { get; set; }

    public override Texture2D Texture2D => Xbox360Buttons;

}