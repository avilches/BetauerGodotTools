using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;

namespace Veronenger.UI.Consoles;

[Singleton]
public class XboxOneControllerSpriteConfig : HyohnooSpriteConfig {
    [Inject("XboxOneButtons")] public IFactory<Texture2D> XboxOneButtons { get; set; }
    public override Texture2D Texture2D => XboxOneButtons.Get();
}