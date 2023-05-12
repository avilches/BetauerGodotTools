using Betauer.Camera;
using Godot;
using Betauer.DI.Attributes;
using static Veronenger.LayerConstants;

namespace Veronenger.Managers; 

[Singleton]
public class StageManager {
    private readonly StageController _stageController = new(LayerStageArea);

    // Each player has its own camera and detector
    public StageController Create(Area2D playerDetector) {
        var cameraStageLimiter = new StageController(LayerStageArea);
        cameraStageLimiter.AddTarget(playerDetector);
        return cameraStageLimiter;
    }

    public void ConfigureStage(Area2D stage) {
        _stageController.ConfigureStage(stage);
    }
}