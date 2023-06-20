using Betauer.Application.SplitScreen;
using Betauer.DI;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Game.RTS.HUD;

public partial class RtsHud : CanvasLayer, IInjectable {
    
    [Inject("RtsPlayerHudFactory")] public RtsPlayerHud PlayerHud1 { get; set; }
    [Inject("RtsPlayerHudFactory")] public RtsPlayerHud PlayerHud2 { get; set; }

    public SplitScreenContainer<RtsPlayerHud> SplitScreenContainer;

    public RtsHud() {
        Name = "HUD";
        Layer = CanvasLayerConstants.HudScene;
        SplitScreenContainer = new SplitScreenContainer<RtsPlayerHud>(() => GetTree().Root.ContentScaleSize);
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

    public void Enable() {
        Visible = true;
    }

    public void Disable() {
        Visible = false;
    }
}