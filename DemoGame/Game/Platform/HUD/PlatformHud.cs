using System;
using Betauer.Application.SplitScreen;
using Betauer.DI;
using Betauer.DI.Attributes;
using Godot;
using Veronenger.Game.Platform.Character.Player;
using Veronenger.Game.Platform.Items;

namespace Veronenger.Game.Platform.HUD;

public partial class PlatformHud : CanvasLayer, IInjectable {
    
    [Inject("PlayerHud")] public PlayerHud PlayerHud1 { get; set; }
    [Inject("PlayerHud")] public PlayerHud PlayerHud2 { get; set; }

    public SplitScreenContainer<PlayerHud> SplitScreenContainer;

    public PlatformHud() {
        Name = "HUD";
        Layer = CanvasLayerConstants.HudScene;
        SplitScreenContainer = new SplitScreenContainer<PlayerHud>(() => GetTree().Root.ContentScaleSize);
    }

    public void PostInject() {
        PlayerHud1.Name = "PlayerHud1";
        PlayerHud2.Name = "PlayerHud2";

        // Ignore is needed because the PlayerHUD captures the mouse
        PlayerHud1.MouseFilter = PlayerHud2.MouseFilter = Control.MouseFilterEnum.Ignore;
        AddChild(PlayerHud1);
        AddChild(PlayerHud2);
        SplitScreenContainer.Split1 = PlayerHud1;
        SplitScreenContainer.Split2 = PlayerHud2;
    }

    public void UpdateHealth(PlayerNode playerNode, PlayerHealthEvent he) {
        GetPlayerHud(playerNode).UpdateHealth(he);
    }

    public void UpdateAmount(PlayerNode playerNode, PickableGameObject gameObject) {
        GetPlayerHud(playerNode).UpdateAmount(gameObject);
    }

    public void UpdateInventory(PlayerNode playerNode, PlayerInventoryEvent playerInventoryEvent) {
        GetPlayerHud(playerNode).UpdateInventory(playerInventoryEvent);
    }

    private PlayerHud GetPlayerHud(PlayerNode playerNode) {
        if (playerNode.PlayerMapping.Player >= 2) throw new Exception("Only 2 players supported");
        return playerNode.PlayerMapping.Player == 0 ? PlayerHud1 : PlayerHud2;
    }

    public void Enable() {
        Visible = true;
    }

    public void Disable() {
        Visible = false;
    }
}