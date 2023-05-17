using System.Collections.Generic;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;

namespace Veronenger.Config;

public interface IDynamicConfig { }

[Singleton]
public class ConfigManager : IInjectable {
    [Inject] public Container Container { get; private set; }
    
    [Inject("KnifeMelee")] public KnifeMelee Knife { get; private set; }
    [Inject("MetalbarMelee")] public MetalbarMelee Metalbar { get; private set; }
    [Inject("SlowGun")] public RangeSlowGun SlowGun { get; private set; }
    [Inject("Gun")] public RangeGun Gun { get; private set; }
    [Inject("Shotgun")] public RangeShotgun Shotgun { get; private set; }
    [Inject("MachineGun")] public RangeMachineGun MachineGun { get; private set; }
    
    // public PickableConfig BulletAmmo { get; private set; }
    // public PickableConfig CartridgeAmmo { get; private set; }

    private Dictionary<string, IDynamicConfig> _configs = new();
    private Dictionary<IDynamicConfig, string> _reverseConfig = new();

    public void PostInject() {
        Container.Query<IDynamicConfig>(Lifetime.Singleton)
            .ForEach(provider => {
                var dynamicConfig = (IDynamicConfig)provider.Get();
                _configs[provider.Name!] = dynamicConfig;
                _reverseConfig[dynamicConfig] = provider.Name!;
            });
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

    public string GetConfigName(IDynamicConfig config) => _reverseConfig[config];
    public IDynamicConfig GetConfig(string name) => _configs[name];
}

