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
        public readonly Texture2D? Bullet;
        public float Speed = 2000;
        public float MaxDistance = 800;
        public float TrailLong = 200;
        public float DelayBetweenShots = 0f;
        
        public Range(Texture2D weaponAnimation, Texture2D? bullet, float damage) : base(damage) {
            WeaponAnimation = weaponAnimation;
            Bullet = bullet;
        }         
    }         
}
