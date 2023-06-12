using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.DI.Attributes;
using Godot;
using static Veronenger.Game.Platform.CollisionLayerConstants;

namespace Veronenger.Game.Platform; 

[Singleton]
public class PlatformConfig {
    [Inject] private SceneTree SceneTree { get; set; }

    private const string GROUP_PLATFORMS = "platform";
    private const string GROUP_MOVING_PLATFORMS = "moving_platform";
    private const string GROUP_FALLING_PLATFORMS = "falling_platform";

    public void ConfigurePlatformList(List<PhysicsBody2D> platforms, bool falling = false, bool moving = false) {
        // platforms.ForEach(kb2d => ConfigurePlatform(kb2d, falling, moving));
    }

    public void ConfigurePlatformsCollisions() {
        SceneTree.GetNodesInGroup(GROUP_PLATFORMS).OfType<PhysicsBody2D>().ForEach(ConfigurePlatformCollision);
    }

    public void ConfigurePlatformCollision(PhysicsBody2D platform) {
        platform.CollisionMask = 0;
        platform.CollisionLayer = 0;
        platform.AddToLayer(LayerSolidBody);
    }

    public void RemovePlatformCollision(PhysicsBody2D platform) {
        platform.RemoveFromLayer(LayerSolidBody);
    }

    public void ConfigureTileMapCollision(TileMap tileMap) {
        tileMap.TileSet.SetPhysicsLayerCollisionLayer(0, 0);
        tileMap.TileSet.SetPhysicsLayerCollisionMask(0, 0);
        tileMap.AddToLayer(0, LayerSolidBody);
    }

    // It accepts GodotObject so it can be used from a GetSlideCollision(x).Collider
    public bool IsPlatform(GodotObject? platform) => platform is PhysicsBody2D psb && psb.IsInGroup(GROUP_PLATFORMS);
    public bool IsMovingPlatform(GodotObject? platform) => platform is PhysicsBody2D psb && psb.IsInGroup(GROUP_MOVING_PLATFORMS);
    public bool IsFallingPlatform(GodotObject? platform) => platform is PhysicsBody2D psb && psb.IsInGroup(GROUP_FALLING_PLATFORMS);

}