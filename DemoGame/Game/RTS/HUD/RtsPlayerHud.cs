using Betauer.NodePath;
using Godot;

namespace Veronenger.Game.RTS;

public partial class RtsPlayerHud : Control {
	[NodePath("%HealthBar")] private TextureProgressBar _healthBar;
	// [NodePath("%Inventory")] private Inventory _inventory;
						  /*
	public void UpdateHealth(PlayerHealthEvent he) {
		_healthBar.MinValue = 0;
		_healthBar.MaxValue = he.Max;
		_healthBar.Value = he.ToHealth;
	}

	public void UpdateAmount(PickableGameObject gameObject) {
		_inventory.UpdateAmount(gameObject);
	}

	public void UpdateInventory(PlayerInventoryEvent playerInventoryEvent) {
		_inventory.UpdateInventory(playerInventoryEvent);
	}
	*/
}
