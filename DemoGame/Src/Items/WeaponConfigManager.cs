using Betauer.DI;
using Godot;

namespace Veronenger.Items; 

[Service]
public class WeaponConfigManager {
    [Inject] private Texture2D LeonKnifeAnimationSprite { get; set; }
    [Inject] private Texture2D LeonMetalbarAnimationSprite { get; set; }

    public WeaponConfig.Melee Knife { get; private set; }
    public WeaponConfig.Melee Metalbar { get; private set; }
    public WeaponConfig.Melee None { get; private set; }

    public WeaponConfig.Range Gun { get; private set; }

    [PostInject]
    private void CreateWeapons() {
        Knife = new WeaponConfig.Melee(LeonKnifeAnimationSprite, "Short", 6);
        Metalbar = new WeaponConfig.Melee(LeonMetalbarAnimationSprite, "Long", 10);
        Gun = new WeaponConfig.Range(LeonMetalbarAnimationSprite, 30);
        None = new WeaponConfig.Melee(null, null, 0);
    } 
}

