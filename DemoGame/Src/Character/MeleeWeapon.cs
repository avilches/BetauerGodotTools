using Godot;

namespace Veronenger.Character;

public class MeleeWeapon {
    public readonly Texture2D Resource;
    public readonly string ShapeName;
    public readonly float Damage;

    public MeleeWeapon(Texture2D resource, string shapeName, float damage) {
        Resource = resource;
        ShapeName = shapeName;
        Damage = damage;
    }
}