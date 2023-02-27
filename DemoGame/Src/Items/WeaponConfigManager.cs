using Betauer.DI;
using Godot;
using Veronenger.Character.Enemy;

namespace Veronenger.Items; 

[Service]
public class WeaponConfigManager {
    [Inject] private Texture2D LeonKnifeAnimationSprite { get; set; }
    [Inject] private Texture2D LeonMetalbarAnimationSprite { get; set; }

    public WeaponConfig.Melee Knife { get; private set; }
    public WeaponConfig.Melee Metalbar { get; private set; }

    public WeaponConfig.Range SlowGun { get; private set; }
    public WeaponConfig.Range Gun { get; private set; }
    public WeaponConfig.Range Shotgun { get; private set; }
    public WeaponConfig.Range MachineGun { get; private set; }

    public NpcConfig ZombieConfig = new NpcConfig();

    [PostInject]
    private void CreateWeapons() {
        Knife = new WeaponConfig.Melee(LeonKnifeAnimationSprite, "Short");
        Metalbar = new WeaponConfig.Melee(LeonMetalbarAnimationSprite, "Long");

        MachineGun = new WeaponConfig.Range(null, null, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 3000, TrailLength = 500, RaycastLength = -1,
        };
        Shotgun = new WeaponConfig.Range(null, null, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 2000, TrailLength = 200, RaycastLength = -1,
        };
        Gun = new WeaponConfig.Range(null, null, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 800, TrailLength = 30, RaycastLength = 30,
        };
        SlowGun = new WeaponConfig.Range(null, null, new Vector2(20f, -33.5f)) {
            MaxDistance = 800, Speed = 500, TrailLength = 20, RaycastLength = 30,
        };
    } 
}

