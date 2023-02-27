namespace Veronenger.Persistent;

public interface IItemNode {
    void OnAddToWorld(ItemRepository itemRepository, Item item);
}