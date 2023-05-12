using Betauer.Application.Persistent;
using Veronenger.Config;

namespace Veronenger.Persistent;

public class WeaponMeleeGameObject : WeaponGameObject {
    public override WeaponConfig.Melee Config => (WeaponConfig.Melee)base.Config;
    public int EnemiesPerHit;

    public WeaponMeleeGameObject Configure(WeaponConfig.Melee config, float damageBase, int enemiesPerHit = 1) {
        Config = config;
        DamageBase = damageBase;
        EnemiesPerHit = enemiesPerHit;
        return this;
    }

    public override SaveObject CreateSaveObject() {
        return new WeaponMeleeSaveObject(this);
    }
}

public class WeaponMeleeSaveObject : SaveObject<WeaponMeleeGameObject> {
    public float DamageBase { get; }
    public int EnemiesPerHit { get; }
    
    public WeaponMeleeSaveObject(WeaponMeleeGameObject weapon) : base(weapon) {
        DamageBase = weapon.DamageBase;
        EnemiesPerHit = weapon.EnemiesPerHit;
    }
}