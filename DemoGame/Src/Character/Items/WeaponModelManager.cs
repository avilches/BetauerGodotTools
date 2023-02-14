using Betauer.DI;
using Godot;

namespace Veronenger.Character.Items; 

[Service]
public class WeaponModelManager {
    [Inject] private Texture2D LeonKnifeAnimationSprite { get; set; }
    [Inject] private Texture2D LeonMetalbarAnimationSprite { get; set; }

    public WeaponModel.Melee Knife { get; private set; }
    public WeaponModel.Melee Metalbar { get; private set; }
    public WeaponModel.Melee None { get; private set; }

    [PostInject]
    private void CreateWeapons() {
        Knife = new WeaponModel.Melee(LeonKnifeAnimationSprite, "Short", 6);
        Metalbar = new WeaponModel.Melee(LeonMetalbarAnimationSprite, "Long", 10);
        None = new WeaponModel.Melee(null, null, 0);
    } 
}

