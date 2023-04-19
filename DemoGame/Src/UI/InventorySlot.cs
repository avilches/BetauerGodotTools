using System;
using Betauer.NodePath;
using Godot;
using Veronenger.Persistent;

namespace Veronenger.UI;

public partial class InventorySlot : ColorRect {

	[NodePath("%TextureRect")] public TextureRect TextureRect;

	public void UpdateInventorySlot(PickableItem pickableItem, bool equipped, bool selected) {
		pickableItem.Config.ConfigureInventoryTextureRect?.Invoke((AtlasTexture)TextureRect.Texture);
		TextureRect.Visible = true;
		if (selected && equipped) {
			TextureRect.Modulate = new Color(1f, 1, 0.5f, 0.5f);
		} else if (selected) {
			TextureRect.Modulate = new Color(1f, 1, 0.5f, 0.5f);
		} else if (equipped) {
			TextureRect.Modulate = new Color(1, 1, 1);
		} else {
			TextureRect.Modulate = new Color(1, 1, 1, 0.5f);
		}
	}
}
