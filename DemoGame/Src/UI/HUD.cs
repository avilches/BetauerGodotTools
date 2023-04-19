using Betauer.DI.Attributes;
using Betauer.NodePath;
using Godot;
using Veronenger.Character.Player;
using Veronenger.Managers;

namespace Veronenger.UI;

public partial class HUD : CanvasLayer {
	[NodePath("%HealthBar")] private TextureProgressBar _healthBar;
	[NodePath("%Inventory")] private Inventory _inventory;
	
	[Inject] private EventBus EventBus { get; set; }

	public void UpdateHealth(PlayerUpdateHealthEvent he) {
		_healthBar.MinValue = 0;
		_healthBar.MaxValue = he.Max;
		_healthBar.Value = he.ToHealth;
	}

	public void StartGame() {
		Visible = true;
	}

	public void EndGame() {
		Visible = false;
	}

	public void UpdateInventory(Veronenger.Character.Player.Inventory inventory) {
		_inventory.UpdateInventory(inventory);
	}
}
