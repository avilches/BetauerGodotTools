using Veronenger.Config;

namespace Veronenger.Persistent;

public class WeaponMeleeItem : WeaponItem {
    public override WeaponConfig.Melee Config => (WeaponConfig.Melee)base.Config;
    public int EnemiesPerHit = 2;

    public WeaponMeleeItem Configure(WeaponConfig.Melee config, float damageBase) {
        Config = config;
        DamageBase = damageBase;
        return this;
    }
}