using Veronenger.Persistent;

namespace Veronenger.Character.Player;

public enum PlayerInventoryEventType {
    PickUp, Drop, Equip
}

public class PlayerInventorySlotEvent {
    public Inventory Inventory { get; init; }
    public PlayerInventoryEventType PlayerInventoryEventType { get; init; }
    public PickableItem PickableItem { get; init; }

    public PlayerInventorySlotEvent(Inventory inventory, PlayerInventoryEventType playerInventoryEventType, PickableItem pickableItem) {
        Inventory = inventory;
        PlayerInventoryEventType = playerInventoryEventType;
        PickableItem = pickableItem;
    }
}

public class PlayerInventoryEvent {
    public Inventory Inventory { get; init; }

    public PlayerInventoryEvent(Inventory inventory) {
        Inventory = inventory;
    }
}