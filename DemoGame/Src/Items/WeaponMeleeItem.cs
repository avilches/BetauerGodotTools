namespace Veronenger.Items;

public class WeaponMeleeItem : WeaponItem {
    public readonly WeaponConfig.Melee Config;
    public int EnemiesPerHit = 2;

    internal WeaponMeleeItem(int id, string name, string alias, WeaponConfig.Melee config, float damageBase) : base(id, name, alias, damageBase) {
        Config = config;
    }
}