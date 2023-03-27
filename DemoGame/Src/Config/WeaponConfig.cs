using Godot;

namespace Veronenger.Config;

public abstract class WeaponConfig : PickableConfig {
    public Texture2D WeaponAnimation { get; protected set; }

    public class Melee : WeaponConfig {
        public string ShapeName { get; protected set; }

        public Melee(Texture2D weaponAnimation, string shapeName) {
            WeaponAnimation = weaponAnimation;
            ShapeName = shapeName;
        }         
    }         
    
    public class Range : WeaponConfig {
        public Texture2D? Projectile  { get; protected set; }
        public Vector2 ProjectileStartPosition  { get; protected set; }
        
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
