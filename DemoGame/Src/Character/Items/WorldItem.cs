namespace Veronenger.Character.Items;

public class WeaponItem : WorldItem {
    public readonly WeaponType Type;
    public float DamageFactor = 1f;

    public float Damage => Type.Damage * DamageFactor;

    internal WeaponItem(int id, string name, string alias, WeaponType type) : base(id, name, alias) {
        Type = type;
    }
}

public abstract class WorldItem {
    public readonly int Id;
    public readonly string Name;
    public readonly string? Alias;

    public WorldItem(int id, string name, string alias) {
        Id = id;
        Name = name;
        Alias = alias;
    }
}