using Betauer.Camera;
using Godot;
using Betauer.DI.Attributes;
using static Veronenger.LayerConstants;

namespace Veronenger.Managers; 

[Singleton]
public class StageManager {
    private readonly CameraStageLimiter _cameraStageLimiter = new(LayerStageArea);

    public CameraStageLimiter ConfigureStageCamera(Camera2D camera2D, Area2D playerDetector) {
        var cameraStageLimiter = new CameraStageLimiter(LayerStageArea);
        cameraStageLimiter.ConfigureStageCamera(camera2D, playerDetector);
        return cameraStageLimiter;
    }

    public void ConfigureStage(Area2D stage) {
        _cameraStageLimiter.ConfigureStage(stage);
    }
}