using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;
using Veronenger.Platform.Character.Player;
using Veronenger.Platform.Persistent;

namespace Veronenger.UI;

public partial class Inventory : GridContainer {

	[Inject] public ITransient<InventorySlot> InventorySlotResource { get; set; }

	private bool _initialize = false;
	public override void _Ready() {
		RemoveAllChildrenIfNeeded();
	}

	public void UpdateAmount(PickableGameObject gameObject) {
		var childCount = GetChildCount();
		for (var i = 0; i < childCount; i++) {
			var inventorySlot = GetChild<InventorySlot>(i);
			if (inventorySlot.PickableGameObject == gameObject) {
				inventorySlot.UpdateAmount();
				break;
			}
		}
	}

	public void UpdateInventory(PlayerInventoryEvent playerInventorySlotEvent) {
		RemoveAllChildrenIfNeeded();
		var inventory = playerInventorySlotEvent.Inventory;
		FixSlotSize(inventory.Items.Count);
		
		inventory.Items.ForEach((pickableGameObject, i) => {
			var inventorySlot = GetChild<InventorySlot>(i);
			var selected = inventory.Selected == i;
			var equipped = 
				pickableGameObject is WeaponMeleeGameObject melee && inventory.WeaponMeleeEquipped == melee || 
				pickableGameObject is WeaponRangeGameObject range && inventory.WeaponRangeEquipped == range;
			inventorySlot.UpdateInventorySlot(pickableGameObject, equipped, selected);
		});
	}

	private void FixSlotSize(int size) {
		var childCount = GetChildCount();
		if (childCount > size) {
			// Hide the slots without items
			for (var i = size; i < childCount; i ++) GetChild<InventorySlot>(i).RemoveSlot();
		} else if (childCount < size) {
			while (childCount < size) {
				var inventorySlot = InventorySlotResource.Create();
				AddChild(inventorySlot);
				childCount++;
			}
		}
	}

	private void RemoveAllChildrenIfNeeded() {
		if (_initialize) return;
		_initialize = true;
		// remove all children
		while (GetChildCount() > 0) {
			var child = GetChild(0);
			RemoveChild(child);
			child.QueueFree();
		}
	}
}
