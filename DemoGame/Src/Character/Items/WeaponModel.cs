using Godot;

namespace Veronenger.Character.Items;

public abstract class WeaponModel {
    public float Damage { get; protected set; }

    private WeaponModel(float damage) {
        Damage = damage;
    }


    public class Melee : WeaponModel {
        public readonly Texture2D WeaponAnimation;
        public readonly string ShapeName;

        public Melee(Texture2D weaponAnimation, string shapeName, float damage) : base(damage) {
            WeaponAnimation = weaponAnimation;
            ShapeName = shapeName;
        }         
    }         
    
}
