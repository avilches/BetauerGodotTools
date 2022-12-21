using Betauer.Camera;
using Godot;
using Betauer.DI;
using static Veronenger.LayerConstants;

namespace Veronenger.Managers; 

[Service]
public class StageManager {
    private readonly CameraStageLimiter _cameraStageLimiter = new(LayerPlayerArea2DDetector);

    public void ConfigureStageCamera(Camera2D camera2D, Area2D playerDetector) {
        _cameraStageLimiter.ConfigureStageCamera(camera2D, playerDetector);
    }

    public void ConfigureStage(Area2D stage) {
        _cameraStageLimiter.ConfigureStage(stage);
    }

    public void ClearState() {
        _cameraStageLimiter.ClearState();
    }
}