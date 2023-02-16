using System;
using Godot;

namespace Veronenger.Items;

public abstract class WeaponConfig {
    public float Damage { get; protected set; }

    private WeaponConfig(float damage) {
        Damage = damage;
    }


    public class Melee : WeaponConfig {
        public readonly Texture2D WeaponAnimation;
        public readonly string ShapeName;

        public Melee(Texture2D weaponAnimation, string shapeName, float damage) : base(damage) {
            WeaponAnimation = weaponAnimation;
            ShapeName = shapeName;
        }         
    }         
    
    public class Range : WeaponConfig {
        public readonly Texture2D WeaponAnimation;
        public float Speed = 800;
        public float MaxDistance = 400;
        public float ShootTime = 0.2f;
        
        public Range(Texture2D weaponAnimation, float damage) : base(damage) {
            WeaponAnimation = weaponAnimation;
        }         
    }         
}
