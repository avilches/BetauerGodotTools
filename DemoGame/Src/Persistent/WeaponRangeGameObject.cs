using System;
using System.Text.Json.Serialization;
using Betauer.Application.Persistent;
using Betauer.Core;
using Godot;
using Veronenger.Config;

namespace Veronenger.Persistent;

public class WeaponRangeGameObject : WeaponGameObject {
    private static readonly Random Random = new Pcg.PcgRandom();

    public override WeaponConfig.Range Config => (WeaponConfig.Range)base.Config;
    public AmmoType AmmoType;
    public int EnemiesPerHit;
    public float DelayBetweenShots;
    public bool Auto;
    public int MagazineSize = 0;
    public float Dispersion = (float)Mathf.DegToRad(0.5);

    public int Ammo = 0;

    public float NewRandomDispersion() => Dispersion != 0 ? Random.Range(-Dispersion, Dispersion) : 0;

    public override void New() {
    }

    protected override Type SaveObjectType => typeof(WeaponRangeSaveObject);

    protected override void DoLoad(SaveObject saveObject) {
    }

    public WeaponRangeGameObject Configure(WeaponConfig.Range config, 
        AmmoType ammoType,
        float damageBase, 
        int enemiesPerHit = 1,
        float delayBetweenShots = 0,
        bool auto = false,
        int magazineSize = 8,
        float dispersion = -1,
        int ammo = -1) {
        
        Config = config;
        AmmoType = ammoType;
        DamageBase = damageBase;
        EnemiesPerHit = enemiesPerHit;
        DelayBetweenShots = delayBetweenShots;
        Auto = auto;
        MagazineSize = magazineSize;
        if (dispersion >= 0) Dispersion = dispersion;
        if (ammo >= 0) {
            Ammo = ammo;
        } else {
            Ammo = magazineSize;
        }
        return this;
    }

    public override SaveObject CreateSaveObject() {
        return new WeaponRangeSaveObject(this);
    }
}

public class WeaponRangeSaveObject : SaveObject<WeaponRangeGameObject> {
    [JsonInclude] public float DamageBase { get; set; }
    [JsonInclude] public float DamageFactor { get; set; }
    [JsonInclude] public int EnemiesPerHit { get; set; }
    [JsonInclude] public float DelayBetweenShots { get; set; }
    [JsonInclude] public bool Auto { get; set; }
    [JsonInclude] public int MagazineSize { get; set; }
    [JsonInclude] public float Dispersion { get; set; }
    [JsonInclude] public int Ammo { get; set; }

    public WeaponRangeSaveObject() {
    }

    public WeaponRangeSaveObject(WeaponRangeGameObject weapon) : base(weapon) {
        DamageBase = weapon.DamageBase;
        DamageFactor = weapon.DamageFactor;
        EnemiesPerHit = weapon.EnemiesPerHit;
        DelayBetweenShots = weapon.DelayBetweenShots;
        Auto = weapon.Auto;
        MagazineSize = weapon.MagazineSize;
        Dispersion = weapon.Dispersion;
        Ammo = weapon.Ammo;
    }
}