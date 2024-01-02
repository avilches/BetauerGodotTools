using Betauer.Input.Controller;
using Betauer.Nodes;
using Godot;

namespace Betauer.Camera; 

public class DragCameraController {
    public Camera2D? Camera2D { get; private set; }
    // public float DragSensitivity { get; set; } = 1.5f;
    // public float DragSmoothingSpeed { get; set; } = -1f;

    public bool IsDragging => DragAndDropController.IsDragging;

    // private bool _camera2DSmoothingEnabled;
    // private float _camera2DSmoothingSpeed;
    // private bool _cameraLimitSmoothed;

    public readonly DragAndDropController DragAndDropController = new();


    public DragCameraController() {
        DragAndDropController.OnDrag += OnDrag;
    }

    public void OnDrag(Vector2 offset) {
        offset *= new Vector2(1 / Camera2D!.Zoom.X, 1 / Camera2D!.Zoom.Y);
        Camera2D!.Position -= offset;
    }

    public DragCameraController WithMouseButton(MouseButton mouseButton) {
        DragAndDropController.MouseButton = mouseButton;
        return this;
    }

    public DragCameraController Attach(Camera2D camera2D) {
        NodeManager.MainInstance.RemoveOnDestroy(Camera2D, OnCameraDestroy);
        Camera2D = camera2D;
        NodeManager.MainInstance.OnDestroy(Camera2D, OnCameraDestroy);
        Enable();
        return this;
    }

    public DragCameraController Detach() {
        NodeManager.MainInstance.RemoveOnDestroy(Camera2D, OnCameraDestroy);
        Enable(false);
        Camera2D = null;
        return this;
    }

    private void OnCameraDestroy() {
        NodeManager.MainInstance.OnInput -= DragAndDropController.Handle;
    }

    public DragCameraController Enable(bool enable = true) {
        NodeManager.MainInstance.OnInput -= DragAndDropController.Handle;
        if (enable) NodeManager.MainInstance.OnInput += DragAndDropController.Handle;
        return this;
    }
/*        
        private void RollbackSmoothingSpeed() {
            Camera2D.PositionSmoothingSpeed = _camera2DSmoothingSpeed;
            Camera2D.PositionSmoothingEnabled = _camera2DSmoothingEnabled;
            Camera2D.LimitSmoothed = _cameraLimitSmoothed;
        }

        private void SaveAndChangeSmoothingSpeed() {
            _cameraLimitSmoothed = Camera2D.LimitSmoothed;
            _camera2DSmoothingSpeed = Camera2D.PositionSmoothingSpeed;
            _camera2DSmoothingEnabled = Camera2D.PositionSmoothingEnabled;
            if (DragSmoothingSpeed >= 0f) {
                // Camera2D.LimitSmoothed = false;
                // Camera2D.PositionSmoothingEnabled = true;
                // Camera2D.PositionSmoothingSpeed = DragSmoothingSpeed;
            }
        }
        */
}