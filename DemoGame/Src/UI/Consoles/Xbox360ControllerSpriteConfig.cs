using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;

namespace Veronenger.UI.Consoles;

[Singleton]
public class Xbox360ControllerSpriteConfig : HyohnooSpriteConfig {
    [Inject]
    public IFactory<Texture2D> Xbox360Buttons { get; set; }

    public override Texture2D Texture2D => Xbox360Buttons.Get();

}