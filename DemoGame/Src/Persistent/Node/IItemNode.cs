namespace Veronenger.Persistent.Node;

public interface IItemNode {
    void OnAddToWorld(ItemRepository itemRepository, Item item);
}