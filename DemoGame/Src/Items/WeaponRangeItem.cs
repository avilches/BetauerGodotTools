namespace Veronenger.Items;

public class WeaponRangeItem : WeaponItem {
    public readonly WeaponModel.Range Model;
    public float DamageFactor = 1f;
    public int EnemiesPerHit = 2;

    public override float Damage => Model.Damage * DamageFactor;

    internal WeaponRangeItem(int id, string name, string alias, WeaponModel.Range model) : base(id, name, alias) {
        Model = model;
    }
}