using Godot;

namespace Betauer.Camera; 

public class StageCameraController : StageController {
    private Camera2D? _currentCamera = null!;

    public StageCameraController(int layer) : base(layer) {
        OnChangeStage += UpdateCamera!;
    }

    public Camera2D? CurrentCamera {
        get => _currentCamera;
        set {
            if (_currentCamera == value) return;
            _currentCamera = value;
            if (CurrentStage != null) TriggerChangeState(null, CurrentStage);
        }
    }
    
    private void UpdateCamera(Area2D oldStage, Area2D newStage) {
        if (_currentCamera != null) {
            var rect2 = CreateAbsoluteRect2(newStage);
            _currentCamera.LimitLeft = (int)rect2.Position.X;
            _currentCamera.LimitTop = (int)rect2.Position.Y;
            _currentCamera.LimitRight = (int)rect2.End.X;
            _currentCamera.LimitBottom = (int)rect2.End.Y;
            Logger.Debug($"Camera {rect2.Position} {rect2.End}");
        }
    }


}