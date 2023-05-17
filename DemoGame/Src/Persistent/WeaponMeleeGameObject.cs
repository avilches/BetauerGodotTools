using System;
using System.Text.Json.Serialization;
using Betauer.Application.Persistent;
using Veronenger.Config;

namespace Veronenger.Persistent;

public class WeaponMeleeGameObject : WeaponGameObject {
    public override WeaponConfigMelee Config => (WeaponConfigMelee)base.Config;
    public int EnemiesPerHit;

    public WeaponMeleeGameObject Configure(WeaponConfigMelee config, float damageBase, int enemiesPerHit = 1) {
        Config = config;
        DamageBase = damageBase;
        EnemiesPerHit = enemiesPerHit;
        return this;
    }

    public override WeaponMeleeSaveObject CreateSaveObject() => new WeaponMeleeSaveObject(this);

    public override void OnInitialize() {
    }

    public override void OnLoad(SaveObject saveObject) {
    }
}

public class WeaponMeleeSaveObject : SaveObject<WeaponMeleeGameObject> {
    [JsonInclude] public float DamageBase { get; set; }
    [JsonInclude] public int EnemiesPerHit { get; set; }

    public override string Discriminator() => "WeaponMelee";

    public WeaponMeleeSaveObject() {
    }

    public WeaponMeleeSaveObject(WeaponMeleeGameObject weapon) : base(weapon) {
        DamageBase = weapon.DamageBase;
        EnemiesPerHit = weapon.EnemiesPerHit;
    }
}