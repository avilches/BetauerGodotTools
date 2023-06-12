using Betauer.DI.Attributes;
using Godot;
using Veronenger.Game.Platform;

namespace Veronenger.Game.Items.Config;

public abstract class WeaponConfig : PickableConfig, IDynamicConfig {
    public abstract Texture2D WeaponAnimation();

    [Inject] public PickupSpriteSheet PickupSpriteSheet { get; set; }
}