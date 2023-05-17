using System.Text.Json.Serialization;
using Betauer.Application.Persistent;
using Veronenger.Config;

namespace Veronenger.Persistent;

public class WeaponMeleeGameObject : WeaponGameObject {
    public override WeaponConfigMelee Config => (WeaponConfigMelee)base.Config;
    public override WeaponConfigMelee GetConfig(string name) => (WeaponConfigMelee)base.GetConfig(name);
    public int EnemiesPerHit;

    public override void OnInitialize() {
    }

    public WeaponMeleeGameObject Configure(WeaponConfigMelee config, float damageBase, float damageFactor = 1f, int enemiesPerHit = 1) {
        Config = config;
        DamageBase = damageBase;
        DamageFactor = damageFactor; // from base
        EnemiesPerHit = enemiesPerHit;
        return this;
    }

    public override WeaponMeleeSaveObject CreateSaveObject() => new WeaponMeleeSaveObject(this);

    public override void OnLoad(SaveObject saveObject) {
        WeaponMeleeSaveObject save = (WeaponMeleeSaveObject)saveObject;
        // From base
        DamageFactor = save.DamageFactor;

        DamageBase = save.DamageBase;
        EnemiesPerHit = save.EnemiesPerHit;
        Config = GetConfig(save.ConfigName);
    }
}

public class WeaponMeleeSaveObject : SaveObject<WeaponMeleeGameObject> {
    // From base
    [JsonInclude] public float DamageFactor { get; set; }

    [JsonInclude] public float DamageBase { get; set; }
    [JsonInclude] public int EnemiesPerHit { get; set; }
    [JsonInclude] public string ConfigName { get; set; }

    public override string Discriminator() => "WeaponMelee";

    public WeaponMeleeSaveObject() {
    }

    public WeaponMeleeSaveObject(WeaponMeleeGameObject weapon) : base(weapon) {
        // From base
        DamageFactor = weapon.DamageFactor;
        
        DamageBase = weapon.DamageBase;
        EnemiesPerHit = weapon.EnemiesPerHit;
        ConfigName = weapon.GetConfigName();
    }
}