using Veronenger.Items;

namespace Veronenger.Character.Enemy;

public interface INpcItemNode : IItemNode {
    float DistanceToPlayer();
    bool CanBeAttacked(WeaponItem weapon);
}