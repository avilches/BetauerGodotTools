namespace Veronenger.Persistent.Node;

public interface INpcItemNode : ILinkableItem {
    float DistanceToPlayer();
    bool CanBeAttacked(WeaponItem weapon);
}