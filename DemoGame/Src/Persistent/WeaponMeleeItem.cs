using Veronenger.Config;

namespace Veronenger.Persistent;

public class WeaponMeleeItem : WeaponItem {
    public WeaponConfig.Melee Config { get; private set; }
    public int EnemiesPerHit = 2;

    public WeaponMeleeItem Configure(WeaponConfig.Melee config, float damageBase) {
        Config = config;
        DamageBase = damageBase;
        return this;
    }
}