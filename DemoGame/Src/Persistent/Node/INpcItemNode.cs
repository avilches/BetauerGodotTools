namespace Veronenger.Persistent.Node;

public interface INpcItemNode : IItemNode {
    float DistanceToPlayer();
    bool CanBeAttacked(WeaponItem weapon);
}