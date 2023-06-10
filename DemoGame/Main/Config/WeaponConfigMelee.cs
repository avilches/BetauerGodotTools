using Betauer.Application.Lifecycle;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Main.Config;

public abstract class WeaponConfigMelee : WeaponConfig {
    public string ShapeName;
}

[Singleton("KnifeMelee")]
public class KnifeMelee : WeaponConfigMelee {
    [Inject("LeonKnifeAnimationSprite")] private ResourceHolder<Texture2D> LeonKnifeAnimationSprite { get; set; }
    public override Texture2D WeaponAnimation() => LeonKnifeAnimationSprite.Get();

    public KnifeMelee() {
        ShapeName = "Short";
    }

    public override void ConfigurePickableSprite2D(Sprite2D sprite2D) => PickupSpriteSheet.ConfigurePickups(sprite2D, 0, 0);
    public override void ConfigureInventoryTextureRect(AtlasTexture atlasTexture) => PickupSpriteSheet.ConfigurePickups(atlasTexture, 0, 1);
}

[Singleton("MetalbarMelee")]
public class MetalbarMelee : WeaponConfigMelee {
    [Inject("LeonMetalbarAnimationSprite")] private ResourceHolder<Texture2D> LeonMetalbarAnimationSprite { get; set; }
    public override Texture2D WeaponAnimation() => LeonMetalbarAnimationSprite.Get();

    public MetalbarMelee() {
        ShapeName = "Long";
    }

    public override void ConfigurePickableSprite2D(Sprite2D sprite2D) => PickupSpriteSheet.ConfigureBigPickups(sprite2D, 0, 0);
    public override void ConfigureInventoryTextureRect(AtlasTexture atlasTexture) => PickupSpriteSheet.ConfigurePickups(atlasTexture, 0, 3);
}