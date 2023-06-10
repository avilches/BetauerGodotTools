using Veronenger.Persistent;

namespace Veronenger.Character.Player;

public enum PlayerInventoryEventType {
    PickUp, Drop, Equip
}

public class PlayerInventorySlotEvent {
    public Inventory Inventory { get; init; }
    public PlayerInventoryEventType PlayerInventoryEventType { get; init; }
    public PickableGameObject PickableGameObject { get; init; }

    public PlayerInventorySlotEvent(Inventory inventory, PlayerInventoryEventType playerInventoryEventType, PickableGameObject pickableGameObject) {
        Inventory = inventory;
        PlayerInventoryEventType = playerInventoryEventType;
        PickableGameObject = pickableGameObject;
    }
}

public class PlayerInventoryEvent {
    public Inventory Inventory { get; init; }

    public PlayerInventoryEvent(Inventory inventory) {
        Inventory = inventory;
    }
}