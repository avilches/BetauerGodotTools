namespace Veronenger.Persistent;

public abstract class WeaponItem : Item {
    public readonly float DamageBase;
    public float DamageFactor = 1f;
    public float Damage => DamageBase * DamageFactor;

    protected WeaponItem(int id, string name, string alias, float damageBase) : base(id, name, alias) {
        DamageBase = damageBase;
    }
}