using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Config;

public abstract class WeaponConfig : PickableConfig, IDynamicConfig {
    public abstract Texture2D WeaponAnimation();

    [Inject] public PickupSpriteSheet PickupSpriteSheet { get; set; }
}