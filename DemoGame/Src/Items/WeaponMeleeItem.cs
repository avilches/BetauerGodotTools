namespace Veronenger.Items;

public class WeaponMeleeItem : WeaponItem {
    public readonly WeaponConfig.Melee Config;
    public float DamageFactor = 1f;
    public int EnemiesPerHit = 2;

    public override float Damage => Config.Damage * DamageFactor;

    internal WeaponMeleeItem(int id, string name, string alias, WeaponConfig.Melee config) : base(id, name, alias) {
        Config = config;
    }
}