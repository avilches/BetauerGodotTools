using System;
using System.Text.Json.Serialization;
using Betauer.Application.Persistent;
using Betauer.Core;
using Betauer.DI.Attributes;
using Godot;
using Veronenger.Game.Items.Config;

namespace Veronenger.Game.Items;

public class WeaponRangeGameObject : WeaponGameObject {

    [Inject] public Random Random { get; set; }
    
    public override WeaponConfigRange Config => (WeaponConfigRange)base.Config;

    public AmmoType AmmoType;
    public int EnemiesPerHit;
    public float DelayBetweenShots;
    public bool Auto;
    public int MagazineSize = 0;
    public float Dispersion = (float)Mathf.DegToRad(0.5);

    public int Ammo = 0;

    public float NewRandomDispersion() => Dispersion != 0 ? Random.Range(-Dispersion, Dispersion) : 0;

    public override void OnInitialize() {
    }

    public WeaponRangeGameObject Configure(WeaponConfigRange config, 
        AmmoType ammoType,
        float damageBase,
        float damageFactor = 1f,
        int enemiesPerHit = 1,
        float delayBetweenShots = 0,
        bool auto = false,
        int magazineSize = 8,
        float dispersion = -1,
        int ammo = -1) {
        
        Config = config;
        AmmoType = ammoType;
        DamageFactor = damageFactor; // from base
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

    public override WeaponRangeSaveObject CreateSaveObject() => new WeaponRangeSaveObject(this);

    public override void OnLoad(SaveObject saveObject) {
        WeaponRangeSaveObject save = (WeaponRangeSaveObject)saveObject;
        // From base
        DamageFactor = save.DamageFactor;

        AmmoType = save.AmmoType;
        DamageBase = save.DamageBase;
        EnemiesPerHit = save.EnemiesPerHit;
        DelayBetweenShots = save.DelayBetweenShots;
        Auto = save.Auto;
        MagazineSize = save.MagazineSize;
        Dispersion = save.Dispersion;
        Ammo = save.Ammo;
        Config = Container.Resolve<WeaponConfigRange>(save.ConfigName);
    }
}

public class WeaponRangeSaveObject : SaveObject<WeaponRangeGameObject> {
    // From base
    [JsonInclude] public float DamageFactor { get; set; }

    [JsonInclude] public AmmoType AmmoType { get; set; }
    [JsonInclude] public float DamageBase { get; set; }
    [JsonInclude] public int EnemiesPerHit { get; set; }
    [JsonInclude] public float DelayBetweenShots { get; set; }
    [JsonInclude] public bool Auto { get; set; }
    [JsonInclude] public int MagazineSize { get; set; }
    [JsonInclude] public float Dispersion { get; set; }
    [JsonInclude] public int Ammo { get; set; }
    [JsonInclude] public string ConfigName { get; set; }

    [JsonInclude] public Vector2 GlobalPosition { get; set; }
    [JsonInclude] public Vector2 Velocity { get; set; }
    [JsonInclude] public bool PickedUp { get; set; }

    public override string Discriminator() => "WeaponRange";

    public WeaponRangeSaveObject() {
    }

    public WeaponRangeSaveObject(WeaponRangeGameObject weapon) : base(weapon) {
        // From base
        DamageFactor = weapon.DamageFactor;

        AmmoType = weapon.AmmoType;
        DamageBase = weapon.DamageBase;
        EnemiesPerHit = weapon.EnemiesPerHit;
        DelayBetweenShots = weapon.DelayBetweenShots;
        Auto = weapon.Auto;
        MagazineSize = weapon.MagazineSize;
        Dispersion = weapon.Dispersion;
        Ammo = weapon.Ammo;
        ConfigName = weapon.Config.Name;
        
        // Node
        if (weapon.Node != null) {
            GlobalPosition = weapon.Node.GlobalPosition;
            Velocity = weapon.Node.Velocity;
            PickedUp = false;
        } else {
            GlobalPosition = Vector2.Zero;
            Velocity = Vector2.Zero;
            PickedUp = true;
        }

    }
}