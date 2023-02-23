using System;
using Godot;

namespace Veronenger.Items;

public abstract class WeaponConfig {
    public float DamageBase { get; protected set; }

    private WeaponConfig(float damageBase) {
        DamageBase = damageBase;
    }


    public class Melee : WeaponConfig {
        public readonly Texture2D WeaponAnimation;
        public readonly string ShapeName;

        public Melee(Texture2D weaponAnimation, string shapeName, float damageBase) : base(damageBase) {
            WeaponAnimation = weaponAnimation;
            ShapeName = shapeName;
        }         
    }         
    
    public class Range : WeaponConfig {
        public readonly Texture2D WeaponAnimation;
        public readonly Texture2D? Projectile;
        public Vector2 ProjectileStartPosition;

        public float Speed = 2000;
        public float MaxDistance = 800;
        public float TrailLong = 200;
        public float DelayBetweenShots = 0f;
        
        public Range(Texture2D weaponAnimation, Texture2D? projectile, Vector2 projectileStartPosition, float damageBase) : base(damageBase) {
            WeaponAnimation = weaponAnimation;
            Projectile = projectile;
            ProjectileStartPosition = projectileStartPosition;
        }         
    }         
}
