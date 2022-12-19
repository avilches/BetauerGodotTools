using Betauer.Nodes;
using Godot;

namespace Betauer.Input.Controller {
    public class DragCameraController {
        public Camera2D? Camera2D { get; private set; }
        // public float DragSensitivity { get; set; } = 1.5f;
        // public float DragSmoothingSpeed { get; set; } = -1f;

        public bool IsDragging => DragAndDropController.IsDragging;

        // private bool _camera2DSmoothingEnabled;
        // private float _camera2DSmoothingSpeed;
        // private bool _cameraLimitSmoothed;

        private INodeEvent? _nodeEvent;
        public readonly DragAndDropController DragAndDropController;

        public DragCameraController(Camera2D camera2D, InputAction action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
            DragAndDropController = new DragAndDropController(action)
                .AddOnDrag(offset => Camera2D!.Position -= offset);
            Attach(camera2D, pauseMode);
        }
        
        public DragCameraController Attach(Camera2D camera2D, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
            Camera2D = camera2D;
            _nodeEvent?.Destroy();
            _nodeEvent = camera2D.OnInput(DragAndDropController.Handle, pauseMode);
            return this;
        }

        public DragCameraController Detach() {
            _nodeEvent?.Destroy();
            Camera2D = null;
            _nodeEvent = null;
            return this;
        }

        public DragCameraController Enable(bool enable = true) {
            if (enable) _nodeEvent?.Enable();
            else _nodeEvent?.Disable();
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
}