using System;
using System.Text.Json.Serialization;
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

    public override void New() {
    }

    protected override Type SaveObjectType => typeof(WeaponMeleeSaveObject);

    protected override void DoLoad(SaveObject saveObject) {
    }
}

public class WeaponMeleeSaveObject : SaveObject<WeaponMeleeGameObject> {
    [JsonInclude] public float DamageBase { get; set; }
    [JsonInclude] public int EnemiesPerHit { get; set; }

    public WeaponMeleeSaveObject() {
    }

    public WeaponMeleeSaveObject(WeaponMeleeGameObject weapon) : base(weapon) {
        DamageBase = weapon.DamageBase;
        EnemiesPerHit = weapon.EnemiesPerHit;
    }
}