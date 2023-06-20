using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Game.Platform.Items.Config;

public abstract class WeaponConfig : PickableConfig {
    public abstract Texture2D WeaponAnimation();

    [Inject] public PickupSpriteSheet PickupSpriteSheet { get; set; }
}