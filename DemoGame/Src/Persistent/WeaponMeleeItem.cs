using Veronenger.Config;

namespace Veronenger.Persistent;

public class WeaponMeleeItem : WeaponItem {
    public override WeaponConfig.Melee Config => (WeaponConfig.Melee)base.Config;
    public int EnemiesPerHit;

    public WeaponMeleeItem Configure(WeaponConfig.Melee config, float damageBase, int enemiesPerHit = 1) {
        Config = config;
        DamageBase = damageBase;
        EnemiesPerHit = enemiesPerHit;
        return this;
    }
}