using System;
using Betauer.Core;
using Betauer.Input.Controller;
using Betauer.Nodes;
using Godot;

namespace Betauer.Camera;

public class DragCameraController {
    public Camera2D? Camera2D { get; private set; }

    public bool Enabled { get; private set; } = true;
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
        try {
            offset *= new Vector2(1 / Camera2D!.Zoom.X, 1 / Camera2D!.Zoom.Y);
            Camera2D!.Position -= offset;
        } catch (Exception) {
            if (Camera2D != null && !Camera2D.IsInstanceValid()) Detach();
            else throw;
        }
    }

    public DragCameraController WithMouseButton(MouseButton mouseButton) {
        DragAndDropController.MouseButton = mouseButton;
        return this;
    }

    public DragCameraController Attach(Camera2D camera2D) {
        Detach();
        Camera2D = camera2D;
        NodeEventHandler.DefaultInstance.OnInput += Handle;
        return this;
    }

    public DragCameraController Detach() {
        // TODO: memory leak if it's not detached
        NodeEventHandler.DefaultInstance.OnInput -= Handle;
        Camera2D = null;
        return this;
    }

    private void Handle(InputEvent @event) {
        if (Enabled) DragAndDropController.Handle(@event);
    }

    public void Enable(bool enable = true) {
        Enabled = enable;
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