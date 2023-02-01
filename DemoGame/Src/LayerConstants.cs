namespace Veronenger; 

public static class LayerConstants {
    public const int LayerSolidBody = 1;

    public const int LayerPlayerBody = 2;
    public const int LayerPlayerDetectorArea = 3; // this should be a 1px vertical line
    public const int LayerStageArea = 4;
    
    public const int LayerEnemyHurtArea = 10; // Player attack area -> enemy hurt area
    public const int LayerPlayerHurtArea = 11; // Enemy attack area -> player hurt area
}

public static class CanvasLayerConstants {
    public const int MainMenu = 0;
    public const int PauseMenu = 1;
    public const int SettingsMenu = 2;
    public const int BottomBar = 3;
    public const int HudScene = 4;
    public const int ModalBox = 5;
}