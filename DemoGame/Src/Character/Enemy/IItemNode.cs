using Veronenger.Items;

namespace Veronenger.Character.Enemy;

public interface IItemNode {
    void OnAddToWorld(World world, Item item);
}