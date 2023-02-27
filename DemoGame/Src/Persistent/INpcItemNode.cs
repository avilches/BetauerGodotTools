namespace Veronenger.Persistent;

public interface INpcItemNode : IItemNode {
    float DistanceToPlayer();
    bool CanBeAttacked(WeaponItem weapon);
}