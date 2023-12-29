using Veronenger.Game.Platform.Items;

namespace Veronenger.Game.Platform.Character.Player;

public enum PlayerInventoryEventType {
    Add, Remove, Equip, Unequip, SlotAmountUpdate, Refresh
}

public readonly struct PlayerInventoryChangeEvent {
    public Inventory Inventory { get; init; }
    public PlayerInventoryEventType Type { get; init; }
    public PickableGameObject PickableGameObject { get; init; }

    public PlayerInventoryChangeEvent(Inventory inventory, PlayerInventoryEventType type, PickableGameObject pickableGameObject) {
        Inventory = inventory;
        Type = type;
        PickableGameObject = pickableGameObject;
    }
}