using Veronenger.Character.Enemy;

namespace Veronenger.Character.Items;

public abstract class Item {
    public readonly int Id;
    public readonly string Name;
    public readonly string? Alias;

    public Item(int id, string name, string alias) {
        Id = id;
        Name = name;
        Alias = alias;
    }
}

public abstract class BaseWeaponItem : Item {
    public abstract float Damage { get; }

    protected BaseWeaponItem(int id, string name, string alias) : base(id, name, alias) {
    }
}

public class WeaponMeleeItem : BaseWeaponItem {
    public readonly WeaponModel.Melee Model;
    public float DamageFactor = 1f;
    public int EnemiesPerHit = 2;

    public override float Damage => Model.Damage * DamageFactor;

    internal WeaponMeleeItem(int id, string name, string alias, WeaponModel.Melee model) : base(id, name, alias) {
        Model = model;
    }
}

public class EnemyItem : Item {
    public readonly ZombieNode ZombieNode;

    public EnemyItem(int id, string name, string alias, ZombieNode zombieNode) : base(id, name, alias) {
        ZombieNode = zombieNode;
    }
}