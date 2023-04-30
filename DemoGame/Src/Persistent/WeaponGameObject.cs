
using Veronenger.Config;

namespace Veronenger.Persistent;

public abstract class WeaponGameObject : PickableGameObject {
    public override WeaponConfig Config => (WeaponConfig)base.Config;

    public float DamageBase;
    public float DamageFactor = 1f;
    public float Damage => DamageBase * DamageFactor;
}