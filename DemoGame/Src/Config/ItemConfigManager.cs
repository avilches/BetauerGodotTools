using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;

namespace Veronenger.Config; 

[Singleton]
public class ItemConfigManager : IInjectable {
    [Inject] private IFactory<Texture2D> LeonKnifeAnimationSprite { get; set; }
    [Inject] private IFactory<Texture2D> LeonMetalbarAnimationSprite { get; set; }
    [Inject] private IFactory<Texture2D> LeonGun1AnimationSprite { get; set; }
    
    [Inject] private IFactory<Texture2D> MetalbarSprite { get; set; }
    [Inject] private IFactory<Texture2D> SlowGunSprite { get; set; }

    public WeaponConfig.Melee Knife { get; private set; }
    public WeaponConfig.Melee Metalbar { get; private set; }

    public WeaponConfig.Range SlowGun { get; private set; }
    public WeaponConfig.Range Gun { get; private set; }
    public WeaponConfig.Range Shotgun { get; private set; }
    public WeaponConfig.Range MachineGun { get; private set; }

    public NpcConfig ZombieConfig = new NpcConfig();
    // TODO: it shouldn't be a singleton. If its used by other classes different than the Player, it's a bad design!
    [Inject] public PlayerConfig PlayerConfig { get; private set; }

    public void PostInject() {
        Knife = new WeaponConfig.Melee(LeonKnifeAnimationSprite, "Short") {
            Initialize = pickableNode => {
                pickableNode.Sprite.Texture = MetalbarSprite.Get();
                pickableNode.Sprite.Hframes = 3;
                pickableNode.Sprite.Frame = 1;
            },
        };
        Metalbar = new WeaponConfig.Melee(LeonMetalbarAnimationSprite, "Long")  {
            Initialize = pickableNode => {
                pickableNode.Sprite.Texture = MetalbarSprite.Get();
                pickableNode.Sprite.Hframes = 3;
                pickableNode.Sprite.Frame = 1;
            },
        };
        MachineGun = new WeaponConfig.Range(LeonGun1AnimationSprite, null, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 3000, TrailLength = 500, RaycastLength = -1,
            Initialize = pickableNode => {
                pickableNode.Sprite.Texture = SlowGunSprite.Get();
                pickableNode.Sprite.Hframes = 1;
                pickableNode.Sprite.Frame = 0;
            },
        };
        Shotgun = new WeaponConfig.Range(LeonGun1AnimationSprite, null, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 2000, TrailLength = 200, RaycastLength = -1,
            Initialize = pickableNode => {
                pickableNode.Sprite.Texture = SlowGunSprite.Get();
                pickableNode.Sprite.Hframes = 1;
                pickableNode.Sprite.Frame = 0;
            },
        };
        Gun = new WeaponConfig.Range(LeonGun1AnimationSprite, null, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 800, TrailLength = 30, RaycastLength = 30,
            Initialize = pickableNode => {
                pickableNode.Sprite.Texture = SlowGunSprite.Get();
                pickableNode.Sprite.Hframes = 1;
                pickableNode.Sprite.Frame = 0;
            },
        };
        SlowGun = new WeaponConfig.Range(LeonGun1AnimationSprite, null, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 500, TrailLength = 20, RaycastLength = 30,
            Initialize = pickableNode => {
                pickableNode.Sprite.Texture = SlowGunSprite.Get();
                pickableNode.Sprite.Hframes = 1;
                pickableNode.Sprite.Frame = 0;
            },
        };
    } 
}

