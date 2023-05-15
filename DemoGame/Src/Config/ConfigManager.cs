using System.Collections.Generic;
using Betauer.DI;
using Betauer.DI.Attributes;

namespace Veronenger.Config;

[Singleton]
public class ConfigManager : IInjectable {
    [Inject("KnifeMelee")] public KnifeMelee Knife { get; private set; }
    [Inject("MetalbarMelee")] public MetalbarMelee Metalbar { get; private set; }
    [Inject("SlowGun")] public RangeSlowGun SlowGun { get; private set; }
    [Inject("Gun")] public RangeGun Gun { get; private set; }
    [Inject("Shotgun")] public RangeShotgun Shotgun { get; private set; }
    [Inject("MachineGun")] public RangeMachineGun MachineGun { get; private set; }
    // public PickableConfig BulletAmmo { get; private set; }
    // public PickableConfig CartridgeAmmo { get; private set; }

    [Inject] public ZombieConfig ZombieConfig { get; private set; }
    // TODO: it shouldn't be a singleton. If its used by other classes different than the Player, it's a bad design!
    [Inject] public PlayerConfig PlayerConfig { get; private set; }

    private Dictionary<string, WeaponConfigRange> ranges = new();
    private Dictionary<string, WeaponConfigMelee> meles = new();

    public void PostInject() {
        meles["Knife"] = Knife;
        meles["Metalbar"] = Metalbar;
        
        ranges["SlowGun"] = SlowGun;
        ranges["Gun"] = Gun;
        ranges["Shotgun"] = Shotgun;
        ranges["MachineGun"] = MachineGun;
    }


    // public void Ammo() {
    //     BulletAmmo = new PickableConfig {
    //         ConfigurePickableSprite2D = sprite => PickupSpriteSheet.ConfigurePickups(sprite, 2, 0),
    //         ConfigureInventoryTextureRect = texture => PickupSpriteSheet.ConfigurePickups(texture, 2, 0)
    //     };
    //     CartridgeAmmo = new PickableConfig {
    //         ConfigurePickableSprite2D = sprite => PickupSpriteSheet.ConfigurePickups(sprite, 3, 0),
    //         ConfigureInventoryTextureRect = texture => PickupSpriteSheet.ConfigurePickups(texture, 3, 0)
    //     };
    // }
}

