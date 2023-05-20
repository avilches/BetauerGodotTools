using Betauer.Application.Lifecycle;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.UI.Consoles;

[Singleton]
public class XboxOneControllerSpriteConfig : HyohnooSpriteConfig {
    [Inject("XboxOneButtons")] public ResourceHolder<Texture2D> XboxOneButtons { get; set; }
    public override Texture2D Texture2D => XboxOneButtons.Get();
}