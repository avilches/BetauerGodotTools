using System;
using Betauer.Core;
using Godot;
using Veronenger.Config;

namespace Veronenger.Persistent;

public class WeaponRangeItem : WeaponItem {
    private static readonly Random Random = new Pcg.PcgRandom();

    public override WeaponConfig.Range Config => (WeaponConfig.Range)base.Config;
    public int EnemiesPerHit = 1;
    public float DelayBetweenShots = 0f;
    public bool Auto = false;
    public float Dispersion = (float)Mathf.DegToRad(0.5);
    public float NewRandomDispersion() => Dispersion != 0 ? Random.Range(-Dispersion, Dispersion) : 0;

    public WeaponRangeItem Configure(WeaponConfig.Range config, float damageBase) {
        Config = config;
        DamageBase = damageBase;
        return this;
    }
}