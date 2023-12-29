using Betauer.NodePath;
using Godot;
using Veronenger.Game.Platform.Character.Player;

namespace Veronenger.Game.Platform.HUD;

public partial class PlayerHud : Control {
	[NodePath("%HealthBar")] private TextureProgressBar _healthBar;
	[NodePath("%Inventory")] private Inventory _inventory;
	
	public void UpdateHealth(PlayerHealthEvent he) {
		_healthBar.MinValue = 0;
		_healthBar.MaxValue = he.Max;
		_healthBar.Value = he.ToHealth;
	}

	public void UpdateInventory(PlayerInventoryChangeEvent playerInventoryEvent) {
		_inventory.UpdateInventory(playerInventoryEvent);
	}
}
