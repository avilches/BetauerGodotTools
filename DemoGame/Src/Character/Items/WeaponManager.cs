using Betauer.DI;
using Godot;

namespace Veronenger.Character.Items;

[Service]
public class WeaponManager {
    [Inject] private Texture2D LeonKnifeAnimationSprite { get; set; }
    [Inject] private Texture2D LeonMetalbarAnimationSprite { get; set; }

    public WeaponModel Knife;
    public WeaponModel Metalbar;
    public WeaponModel None;

    [PostInject]
    public void CreateWeaponModels() {
        Knife = new WeaponModel(LeonKnifeAnimationSprite, "Short", 6);
        Metalbar = new WeaponModel(LeonMetalbarAnimationSprite, "Long", 10);
        None = new WeaponModel(null, null, 0);
    } 
}

public class WeaponModel {
    public readonly Texture2D Resource;
    public readonly string ShapeName;
    public readonly float Damage;

    public WeaponModel(Texture2D resource, string shapeName, float damage) {
        Resource = resource;
        ShapeName = shapeName;
        Damage = damage;
    }         
}         
