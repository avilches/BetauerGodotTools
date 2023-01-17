using Betauer.DI;
using Godot;

namespace Veronenger.Character.Items;

[Service]
public class WeaponManager {
    [Inject] private Texture2D LeonKnifeAnimationSprite { get; set; }
    [Inject] private Texture2D LeonMetalbarAnimationSprite { get; set; }

    public WeaponType Knife;
    public WeaponType Metalbar;
    public WeaponType None;

    [PostInject]
    public void CreateWeaponTypes() {
        Knife = new WeaponType(LeonKnifeAnimationSprite, "Short", 6);
        Metalbar = new WeaponType(LeonMetalbarAnimationSprite, "Long", 10);
        None = new WeaponType(null, null, 0);
    } 
}

public class WeaponType {
    public readonly Texture2D Resource;
    public readonly string ShapeName;
    public readonly float Damage;

    public WeaponType(Texture2D resource, string shapeName, float damage) {
        Resource = resource;
        ShapeName = shapeName;
        Damage = damage;
    }         
}         
