namespace Veronenger.Items;

public abstract class WeaponItem : Item {
    public abstract float Damage { get; }

    protected WeaponItem(int id, string name, string alias) : base(id, name, alias) {
    }
}