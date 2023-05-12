using Betauer.Core.Signal;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;
using Veronenger.Character.Player;
using Veronenger.Persistent;

namespace Veronenger.UI;

[Singleton]
public partial class HUD : CanvasLayer {

	public HBoxContainer SplitScreen = new();

	public PlayerHud PlayerHud1;
	public PlayerHud PlayerHud2;

	[Inject] public IFactory<PlayerHud> PlayerHudFactory { get; set; }
	
	public void Configure() {
		PlayerHud1 = PlayerHudFactory.Get();
		PlayerHud2 = PlayerHudFactory.Get();
		AddChild(SplitScreen);
		PlayerHud1.Name = "PlayerHud1";
		PlayerHud2.Name = "PlayerHud2";
		SplitScreen.Name = "SplitScreen";
		SplitScreen.AddChild(PlayerHud1);
		SplitScreen.AddChild(PlayerHud2);
		Name = "HUD";        
		Visible = false;
	}

	public void UpdateHealth(PlayerNode playerNode, PlayerHealthEvent he) {
		GetPlayerHud(playerNode).UpdateHealth(he);
	}

	private PlayerHud GetPlayerHud(PlayerNode playerNode) {
		if (playerNode.PlayerMapping.Player >= 2) throw new System.Exception("Only 2 players supported");
		return playerNode.PlayerMapping.Player == 0 ? PlayerHud1 : PlayerHud2;
	}

	public void Enable() {
		Visible = true;
	}

	public void Disable() {
		Visible = false;
	}

	public void UpdateAmount(PlayerNode playerNode, PickableGameObject gameObject) {
		GetPlayerHud(playerNode).UpdateAmount(gameObject);
	}

	public void UpdateInventory(PlayerNode playerNode, PlayerInventoryEvent playerInventoryEvent) {
		GetPlayerHud(playerNode).UpdateInventory(playerInventoryEvent);
	}

	public void EnablePlayer2() {
		var windowSize = GetTree().Root.ContentScaleSize;
		var halfScreen = new Vector2((float)windowSize.X / 2, windowSize.Y);
		PlayerHud1.CustomMinimumSize = halfScreen;
		PlayerHud2.CustomMinimumSize = halfScreen;
		PlayerHud1.Visible = true;
		PlayerHud2.Visible = true;
	}

	public void DisablePlayer2() {
		var windowSize = GetTree().Root.ContentScaleSize;
		PlayerHud1.CustomMinimumSize = windowSize;
		PlayerHud1.Visible = true;
		PlayerHud2.Visible = false;
	}
}
