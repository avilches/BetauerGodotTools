using System;
using Betauer.Core;
using Godot;

namespace Veronenger.Items;

public class WeaponRangeItem : WeaponItem {
    private static readonly Random Random = new Pcg.PcgRandom();

    public readonly WeaponConfig.Range Config;
    public float DamageFactor = 1f;
    public int EnemiesPerHit = 1;
    public float DelayBetweenShots = 0f;
    public bool Auto = false;

    public override float Damage => Config.DamageBase * DamageFactor;
    public float Dispersion = (float)Mathf.DegToRad(0.5);

    public float NewRandomDispersion() => Dispersion != 0 ? Random.Range(-Dispersion, Dispersion) : 0;

    internal WeaponRangeItem(int id, string name, string alias, WeaponConfig.Range config) : base(id, name, alias) {
        Config = config;
    }
}