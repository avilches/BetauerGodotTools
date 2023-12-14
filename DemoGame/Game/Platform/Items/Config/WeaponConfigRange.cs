using Betauer.Application.Lifecycle;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Game.Platform.Items.Config;

public abstract class WeaponConfigRange : WeaponConfig {
    public Texture2D? Projectile { get; protected set; }
    public Vector2 ProjectileStartPosition { get; protected set; }
    [Inject("LeonGun1AnimationSprite")] protected ResourceHolder<Texture2D> LeonGun1AnimationSprite { get; set; }
    
    // Name is used to save the config service in the savegame. It should match with the singleton name
    public readonly string Name;
    protected WeaponConfigRange(string name) {
        Name = name;
    }

    public int MaxDistance = 800;
    public int Speed = 2000;
    public int TrailLength = 200;
    public int RaycastLength = -1;
}

[Singleton(Name = "SlowGun")]
public class RangeSlowGun : WeaponConfigRange {
    public override Texture2D WeaponAnimation() => LeonGun1AnimationSprite.Get();

    public RangeSlowGun() : base("SlowGun") {
        ProjectileStartPosition = new Vector2(20f, -33.5f);
        MaxDistance = 800;
        Speed = 500;
        TrailLength = 20;
        RaycastLength = 30;
    }

    public override void ConfigurePickableSprite2D(Sprite2D sprite2D) => PickupSpriteSheet.ConfigurePickups(sprite2D, 4, 1);
    public override void ConfigureInventoryTextureRect(AtlasTexture atlasTexture) => PickupSpriteSheet.ConfigurePickups(atlasTexture, 4, 1);
}

[Singleton(Name = "Gun")]
public class RangeGun : WeaponConfigRange {
    public override Texture2D WeaponAnimation() => LeonGun1AnimationSprite.Get();

    public RangeGun() : base("Gun") {
        ProjectileStartPosition = new Vector2(20f, -33.5f);
        MaxDistance = 800;
        Speed = 800;
        TrailLength = 30;
        RaycastLength = 30;
    }

    public override void ConfigurePickableSprite2D(Sprite2D sprite2D) => PickupSpriteSheet.ConfigurePickups(sprite2D, 4, 1);
    public override void ConfigureInventoryTextureRect(AtlasTexture atlasTexture) => PickupSpriteSheet.ConfigurePickups(atlasTexture, 4, 1);
}

[Singleton(Name = "Shotgun")]
public class RangeShotgun : WeaponConfigRange {
    public override Texture2D WeaponAnimation() => LeonGun1AnimationSprite.Get();

    public RangeShotgun() : base("Shotgun") {
        ProjectileStartPosition = new Vector2(20f, -33.5f);
        MaxDistance = 800;
        Speed = 2000;
        TrailLength = 200;
        RaycastLength = -1;
    }

    public override void ConfigurePickableSprite2D(Sprite2D sprite2D) => PickupSpriteSheet.ConfigurePickups(sprite2D, 4, 1);
    public override void ConfigureInventoryTextureRect(AtlasTexture atlasTexture) => PickupSpriteSheet.ConfigurePickups(atlasTexture, 4, 1);
}

[Singleton(Name = "MachineGun")]
public class RangeMachineGun : WeaponConfigRange {
    public override Texture2D WeaponAnimation() => LeonGun1AnimationSprite.Get();

    public RangeMachineGun() : base("MachineGun") {
        ProjectileStartPosition = new Vector2(20f, -33.5f);
        MaxDistance = 800;
        Speed = 3000;
        TrailLength = 500;
        RaycastLength = -1;
    }

    public override void ConfigurePickableSprite2D(Sprite2D sprite2D) => PickupSpriteSheet.ConfigurePickups(sprite2D, 4, 1);
    public override void ConfigureInventoryTextureRect(AtlasTexture atlasTexture) => PickupSpriteSheet.ConfigurePickups(atlasTexture, 4, 1);
}