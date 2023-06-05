using Betauer.NodePath;
using Godot;
using Veronenger.Platform.Persistent;

namespace Veronenger.UI;

public partial class InventorySlot : ColorRect {

	[NodePath("%TextureRect")] public TextureRect TextureRect;
	[NodePath("%Amount")] public Label Amount;

	public PickableGameObject PickableGameObject;

	public void RemoveSlot() {
		Visible = false;
		PickableGameObject = null;
	}

	public void UpdateInventorySlot(PickableGameObject pickableGameObject, bool equipped, bool selected) {
		PickableGameObject = pickableGameObject;
		Visible = true;
		pickableGameObject.Config.ConfigureInventoryTextureRect((AtlasTexture)TextureRect.Texture);
		TextureRect.Visible = true;
		UpdateAmount();
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

	public void UpdateAmount() {
		if (PickableGameObject is WeaponRangeGameObject range) {
			Amount.Text = range.Ammo.ToString();
		} else {
			Amount.Text = "";
		}
	}
}
