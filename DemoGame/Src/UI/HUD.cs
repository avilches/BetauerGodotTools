using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.NodePath;
using Godot;
using Veronenger.Character.Player;
using Veronenger.Managers;
using Veronenger.Persistent;

namespace Veronenger.UI;

public partial class HUD : CanvasLayer {
	[NodePath("%HealthBar")] private TextureProgressBar _healthBar;
	[NodePath("%Inventory")] private GridContainer _slots;
	
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

	public void UpdateInventory(Inventory inventory) {
		FixSlotSize(inventory.Items.Count);
		
		inventory.Items.ForEach((worldItem, i) => {
			var textureRect = _slots.GetChild<TextureRect>(i);
			var selected = inventory.Selected == i;
			var equipped = 
				worldItem is WeaponMeleeItem melee && inventory.WeaponMeleeEquipped == melee || 
				worldItem is WeaponRangeItem range && inventory.WeaponRangeEquipped == range; 
			UpdateInventorySlot(worldItem, textureRect, equipped, selected);
		});
	}

	private void FixSlotSize(int size) {
		var childCount = _slots.GetChildCount();
		if (childCount > size) {
			// Hide the slots without items
			for (var i = size; i < childCount; i ++) _slots.GetChild<TextureRect>(i).Visible = false;
		} else if (childCount < size) {
			var source = _slots.GetChild<TextureRect>(0);
			while (childCount < size) {
				var duplicate = (TextureRect)source.Duplicate();
				var texture = (AtlasTexture)source.Texture.Duplicate();
				duplicate.Texture = texture;
				_slots.AddChild(duplicate);
				childCount++;
			}
		}
	}

	private void UpdateInventorySlot(PickableItem pickableItem, TextureRect textureRect, bool equipped, bool selected) {
		pickableItem.Config.ConfigureInventoryTextureRect?.Invoke((AtlasTexture)textureRect.Texture);
		textureRect.Visible = true;
		if (selected && equipped) {
			textureRect.Modulate = new Color(1f, 1, 0.5f, 0.5f);
		} else if (selected) {
			textureRect.Modulate = new Color(1f, 1, 0.5f, 0.5f);
		} else if (equipped) {
			textureRect.Modulate = new Color(1, 1, 1);
		} else {
			textureRect.Modulate = new Color(1, 1, 1, 0.5f);
		}
	}
}
