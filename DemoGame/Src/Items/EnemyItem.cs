using Veronenger.Character.Enemy;

namespace Veronenger.Items;

public class EnemyItem : Item {
    public readonly ZombieNode ZombieNode;

    public EnemyItem(int id, string name, string alias, ZombieNode zombieNode) : base(id, name, alias) {
        ZombieNode = zombieNode;
    }
}