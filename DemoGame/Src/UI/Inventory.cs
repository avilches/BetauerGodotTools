using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;
using Veronenger.Character.Player;
using Veronenger.Persistent;

namespace Veronenger.UI;

public partial class Inventory : GridContainer {

	[Inject] public IFactory<InventorySlot> InventorySlotResource { get; set; }

	private bool _initialize = false;
	public override void _Ready() {
		RemoveAllChildrenIfNeeded();
	}

	public void UpdateAmount(PickableItem item) {
		var childCount = GetChildCount();
		for (var i = 0; i < childCount; i++) {
			var inventorySlot = GetChild<InventorySlot>(i);
			if (inventorySlot.PickableItem == item) {
				inventorySlot.UpdateAmount();
				break;
			}
		}
	}

	public void UpdateInventory(PlayerInventoryEvent playerInventorySlotEvent) {
		RemoveAllChildrenIfNeeded();
		var inventory = playerInventorySlotEvent.Inventory;
		FixSlotSize(inventory.Items.Count);
		
		inventory.Items.ForEach((worldItem, i) => {
			var inventorySlot = GetChild<InventorySlot>(i);
			var selected = inventory.Selected == i;
			var equipped = 
				worldItem is WeaponMeleeItem melee && inventory.WeaponMeleeEquipped == melee || 
				worldItem is WeaponRangeItem range && inventory.WeaponRangeEquipped == range;
			inventorySlot.UpdateInventorySlot(worldItem, equipped, selected);
		});
	}

	private void FixSlotSize(int size) {
		var childCount = GetChildCount();
		if (childCount > size) {
			// Hide the slots without items
			for (var i = size; i < childCount; i ++) GetChild<InventorySlot>(i).RemoveSlot();
		} else if (childCount < size) {
			while (childCount < size) {
				var inventorySlot = InventorySlotResource.Get();
				inventorySlot.AddTo(this);
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
