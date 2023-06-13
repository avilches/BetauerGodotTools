using Betauer.DI;
using Betauer.DI.Attributes;
using Godot;
using Veronenger.Game.Character.Player;
using Veronenger.Game.Items;

namespace Veronenger.Game.HUD;

public partial class HudCanvas : CanvasLayer, IInjectable {

	public HBoxContainer SplitScreen = new();

	[Inject("PlayerHudFactory")] public PlayerHud PlayerHud1 { get; set; }
	[Inject("PlayerHudFactory")] public PlayerHud PlayerHud2 { get; set; }

	public int VisiblePlayers { get; private set; } = 0;

	public void PostInject() {
		AddChild(SplitScreen);
		PlayerHud1.Name = "PlayerHud1";
		PlayerHud2.Name = "PlayerHud2";
		SplitScreen.Name = "SplitScreen";
		SplitScreen.AddChild(PlayerHud1);
		SplitScreen.AddChild(PlayerHud2);
		Name = "HUD";        
		Layer = CanvasLayerConstants.HudScene;
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

	public void SinglePlayer() {
		VisiblePlayers = 1;
		var windowSize = GetTree().Root.ContentScaleSize;
		PlayerHud1.CustomMinimumSize = windowSize;
		PlayerHud1.Visible = true;
		PlayerHud2.Visible = false;
	}

	public void EnableSplitScreen() {
		VisiblePlayers = 2;
		var windowSize = GetTree().Root.ContentScaleSize;
		var halfScreen = new Vector2((float)windowSize.X / 2, windowSize.Y);
		PlayerHud1.CustomMinimumSize = halfScreen;
		PlayerHud2.CustomMinimumSize = halfScreen;
		PlayerHud1.Visible = true;
		PlayerHud2.Visible = true;
	}
}
