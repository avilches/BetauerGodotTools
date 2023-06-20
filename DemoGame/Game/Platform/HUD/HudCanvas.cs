using System;
using Betauer.Application.SplitScreen;
using Betauer.DI;
using Betauer.DI.Attributes;
using Godot;
using Veronenger.Game.Platform.Character.Player;
using Veronenger.Game.Platform.Items;

namespace Veronenger.Game.Platform.HUD;

public partial class HudCanvas : CanvasLayer, IInjectable {
    [Inject("PlayerHudFactory")] public PlayerHud Split1 { get; set; }
    [Inject("PlayerHudFactory")] public PlayerHud Split2 { get; set; }

    public SplitScreenContainer<PlayerHud> SplitScreenContainer;

    public void PostInject() {
        Split1.Name = "PlayerHud1";
        Split2.Name = "PlayerHud2";
        AddChild(Split1);
        AddChild(Split2);
        Name = "HUD";
        Layer = CanvasLayerConstants.HudScene;
        SplitScreenContainer = new SplitScreenContainer<PlayerHud>() {
            Split1 = Split1,
            Split2 = Split2,
            Horizontal = true,
            GetSize = () => GetTree().Root.ContentScaleSize
        };
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
        return playerNode.PlayerMapping.Player == 0 ? Split1 : Split2;
    }

    public void Enable() {
        Visible = true;
    }

    public void Disable() {
        Visible = false;
    }
}