namespace Veronenger.Items;

public class WeaponMeleeItem : WeaponItem {
    public readonly WeaponModel.Melee Model;
    public float DamageFactor = 1f;
    public int EnemiesPerHit = 2;

    public override float Damage => Model.Damage * DamageFactor;

    internal WeaponMeleeItem(int id, string name, string alias, WeaponModel.Melee model) : base(id, name, alias) {
        Model = model;
    }
}