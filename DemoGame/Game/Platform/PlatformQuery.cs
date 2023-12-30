using Betauer.DI.Attributes;
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
}