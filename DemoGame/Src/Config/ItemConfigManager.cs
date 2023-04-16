using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;
using Veronenger.Worlds;

namespace Veronenger.Config; 

[Singleton]
public class ItemConfigManager : IInjectable {
    [Inject] private IFactory<Texture2D> LeonKnifeAnimationSprite { get; set; }
    [Inject] private IFactory<Texture2D> LeonMetalbarAnimationSprite { get; set; }
    [Inject] private IFactory<Texture2D> LeonGun1AnimationSprite { get; set; }
    
    [Inject] private IFactory<Texture2D> Pickups { get; set; }
    [Inject] private IFactory<Texture2D> Pickups2 { get; set; }

    public WeaponConfig.Melee Knife { get; private set; }
    public WeaponConfig.Melee Metalbar { get; private set; }

    public WeaponConfig.Range SlowGun { get; private set; }
    public WeaponConfig.Range Gun { get; private set; }
    public WeaponConfig.Range Shotgun { get; private set; }
    public WeaponConfig.Range MachineGun { get; private set; }
    
    public PickableConfig BulletAmmo { get; private set; }
    public PickableConfig CartridgeAmmo { get; private set; }

    public NpcConfig ZombieConfig = new NpcConfig();
    // TODO: it shouldn't be a singleton. If its used by other classes different than the Player, it's a bad design!
    [Inject] public PlayerConfig PlayerConfig { get; private set; }

    public void PostInject() {
        RangeWeapons();
        MeleeWeapons();
        Ammo();
    }

    public void MeleeWeapons() {
        Knife = new WeaponConfig.Melee(LeonKnifeAnimationSprite, "Short") {
            Initialize = pickableNode => ConfigurePickups(pickableNode, 0, 1),
        };
        Metalbar = new WeaponConfig.Melee(LeonMetalbarAnimationSprite, "Long") {
            Initialize = pickableNode => ConfigurePickups2(pickableNode, 1, 0),
        };
    }

    public void RangeWeapons() {
        MachineGun = new WeaponConfig.Range(LeonGun1AnimationSprite, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 3000, TrailLength = 500, RaycastLength = -1,
            Initialize = pickableNode => ConfigurePickups(pickableNode, 4, 1),
        };
        Shotgun = new WeaponConfig.Range(LeonGun1AnimationSprite, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 2000, TrailLength = 200, RaycastLength = -1,
            Initialize = pickableNode => ConfigurePickups(pickableNode, 4, 1),
        };
        Gun = new WeaponConfig.Range(LeonGun1AnimationSprite, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 800, TrailLength = 30, RaycastLength = 30,
            Initialize = pickableNode => ConfigurePickups(pickableNode, 4, 1),
        };
        SlowGun = new WeaponConfig.Range(LeonGun1AnimationSprite, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 500, TrailLength = 20, RaycastLength = 30,
            Initialize = pickableNode => ConfigurePickups(pickableNode, 4, 1),
        };
    }

    public void Ammo() {
        BulletAmmo = new PickableConfig { Initialize = pickableNode => ConfigurePickups(pickableNode, 2, 0) };
        CartridgeAmmo = new PickableConfig { Initialize = pickableNode => ConfigurePickups(pickableNode, 3, 0) };
    }

    public void ConfigurePickups(PickableItemNode pickableNode, int row, int column) => 
        ConfigurePickups(pickableNode, row * 6 + column);

    public void ConfigurePickups(PickableItemNode pickableNode, int frame) {
        pickableNode.Sprite.Texture = Pickups.Get();
        pickableNode.Sprite.Hframes = 6;
        pickableNode.Sprite.Vframes = 6;
        pickableNode.Sprite.Frame = frame;
    }

    public void ConfigurePickups2(PickableItemNode pickableNode, int row, int column) => 
        ConfigurePickups2(pickableNode, row * 3 + column);

    public void ConfigurePickups2(PickableItemNode pickableNode, int frame) {
        pickableNode.Sprite.Texture = Pickups2.Get();
        pickableNode.Sprite.Hframes = 3;
        pickableNode.Sprite.Vframes = 6;
        pickableNode.Sprite.Frame = frame;
    }
}

