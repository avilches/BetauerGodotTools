using Betauer.DI;
using Godot;

namespace Veronenger.Character;

[Service]
public class WeaponManager {
    [Inject] private Texture2D LeonKnife { get; set; }
    [Inject] private Texture2D LeonMetalbar { get; set; }

    public MeleeWeapon Knife;
    public MeleeWeapon Metalbar;
    public MeleeWeapon None;

    [PostInject]
    public void CreateWeapons() {
        Knife = new MeleeWeapon(LeonKnife, "Short", 6);
        Metalbar = new MeleeWeapon(LeonMetalbar, "Long", 10);
        None = new MeleeWeapon(null, null, 0);
    } 
}