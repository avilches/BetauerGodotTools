namespace Veronenger.Game.Platform; 

public static class CollisionLayerConstants {
    public const int LayerSolidBody = 1;

    public const int LayerPlayerBody = 2;
    public const int LayerPlayerDetectorArea = 3; // this should be a 1px vertical line
    public const int LayerStageArea = 4;
    public const int LayerPickableArea = 6;
    
    public const int LayerEnemyHurtArea = 10; // Player attack area -> enemy hurt area
    public const int LayerPlayerHurtArea = 11; // Enemy attack area -> player hurt area
}