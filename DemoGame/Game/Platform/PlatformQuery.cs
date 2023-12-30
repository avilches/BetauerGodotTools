using Betauer.DI.Attributes;
using Godot;
using Veronenger.Game.Platform.Character.Player;
using Veronenger.Game.Platform.Items;
using Veronenger.Game.Platform.World;

namespace Veronenger.Game.Platform;

[Singleton]
public class PlatformQuery {
    private PlatformWorld _platformWorld;
    
    public void Configure(PlatformWorld platformWorld) {
        _platformWorld = platformWorld;
    }
    
    public ProjectileTrail NewBullet() => _platformWorld.NewBullet();

    public PlayerNode FindClosestPlayer(Vector2 origin) => _platformWorld.FindClosestPlayer(origin);
}