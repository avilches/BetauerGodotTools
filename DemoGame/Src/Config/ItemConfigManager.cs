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
    
    private const int PickupItemW = 16;
    private const int PickupItemH = 16;
    private const int PickupColumns = 6;
    private const int PickupRows = 6;
    [Inject] private IFactory<Texture2D> Pickups { get; set; }

    private const int Pickup2ItemW = 32;
    private const int Pickup2ItemH = 16;
    private const int Pickup2Columns = 3;
    private const int Pickup2Rows = 6;
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
            ConfigurePickableSprite2D = sprite => ConfigurePickups(sprite, 0, 1),
            ConfigureInventoryTextureRect = texture => ConfigurePickups(texture, 0, 1),
        };
        Metalbar = new WeaponConfig.Melee(LeonMetalbarAnimationSprite, "Long") {
            ConfigurePickableSprite2D = sprite => ConfigureBigPickups(sprite, 1, 0),
            ConfigureInventoryTextureRect = texture => ConfigurePickups(texture, 0, 3),
        };
    }

    public void RangeWeapons() {
        MachineGun = new WeaponConfig.Range(LeonGun1AnimationSprite, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 3000, TrailLength = 500, RaycastLength = -1,
            ConfigurePickableSprite2D = sprite => ConfigurePickups(sprite, 4, 1),
            ConfigureInventoryTextureRect = texture => ConfigurePickups(texture, 4, 1),
        };
        Shotgun = new WeaponConfig.Range(LeonGun1AnimationSprite, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 2000, TrailLength = 200, RaycastLength = -1,
            ConfigurePickableSprite2D = sprite => ConfigurePickups(sprite, 4, 1),
            ConfigureInventoryTextureRect = texture => ConfigurePickups(texture, 4, 1),
        };
        Gun = new WeaponConfig.Range(LeonGun1AnimationSprite, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 800, TrailLength = 30, RaycastLength = 30,
            ConfigurePickableSprite2D = sprite => ConfigurePickups(sprite, 4, 1),
            ConfigureInventoryTextureRect = texture => ConfigurePickups(texture, 4, 1),
        };
        SlowGun = new WeaponConfig.Range(LeonGun1AnimationSprite, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 500, TrailLength = 20, RaycastLength = 30,
            ConfigurePickableSprite2D = sprite => ConfigurePickups(sprite, 4, 1),
            ConfigureInventoryTextureRect = texture => ConfigurePickups(texture, 4, 1),
        };
    }

    public void Ammo() {
        BulletAmmo = new PickableConfig {
            ConfigurePickableSprite2D = sprite => ConfigurePickups(sprite, 2, 0),
            ConfigureInventoryTextureRect = texture => ConfigurePickups(texture, 2, 0)
        };
        CartridgeAmmo = new PickableConfig {
            ConfigurePickableSprite2D = sprite => ConfigurePickups(sprite, 3, 0),
            ConfigureInventoryTextureRect = texture => ConfigurePickups(texture, 3, 0)
        };
    }

    public void ConfigurePickups(AtlasTexture texture, int row, int column) {
        texture.Region = new Rect2(column * PickupItemW, row * PickupItemH, PickupItemW, PickupItemH);
    }

    public void ConfigurePickups(Sprite2D sprite, int row, int column) {
        var frame = row * PickupColumns + column;
        sprite.Texture = Pickups.Get();
        sprite.Hframes = PickupColumns;
        sprite.Vframes = PickupRows;
        sprite.Frame = frame;
    }

    public void ConfigureBigPickups(Sprite2D sprite, int row, int column) { 
        var frame = row * Pickup2Columns + column;
        sprite.Texture = Pickups2.Get();
        sprite.Hframes = Pickup2Columns;
        sprite.Vframes = Pickup2Rows;
        sprite.Frame = frame;
    }
}

