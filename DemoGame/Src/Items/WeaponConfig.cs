using System;
using Godot;

namespace Veronenger.Items;

public abstract class WeaponConfig {
    public class Melee : WeaponConfig {
        public readonly Texture2D WeaponAnimation;
        public readonly string ShapeName;

        public Melee(Texture2D weaponAnimation, string shapeName) {
            WeaponAnimation = weaponAnimation;
            ShapeName = shapeName;
        }         
    }         
    
    public class Range : WeaponConfig {
        public readonly Texture2D WeaponAnimation;
        public readonly Texture2D? Projectile;
        public Vector2 ProjectileStartPosition;

        public int MaxDistance = 800;
        public int Speed = 2000;
        public int TrailLength = 200;
        public int RaycastLength = -1;
        
        public Range(Texture2D weaponAnimation, Texture2D? projectile, Vector2 projectileStartPosition) {
            WeaponAnimation = weaponAnimation;
            Projectile = projectile;
            ProjectileStartPosition = projectileStartPosition;
        }         
    }         
}
