using System;
using Betauer.Input;
using Godot;

namespace Betauer.Application.Camera {
    public class DragCameraController {
        public Camera2D Camera2D;
        public MouseButton Button;
        public float DragSensitivity;
        public float DragSmoothingSpeed = 1f;
        public bool Enabled = true;
        /// <summary>
        ///  Where bool params true means start, and false means ends
        /// </summary>
        public Action<bool>? OnDrag;

        public bool IsDragging => _startDragCameraPosition.HasValue;

        private Vector2? _startDragCameraPosition;
        private float _camera2DSmoothingSpeed;

        public DragCameraController(Camera2D camera2D, MouseButton button = MouseButton.Middle, float dragSensitivity = 1.5f, float dragSmoothingSpeed = -1f) {
            Camera2D = camera2D;
            Button = button;
            DragSensitivity = dragSensitivity;
            DragSmoothingSpeed = dragSmoothingSpeed;
        }

        public bool DragCamera(InputEvent e) {
            if (!Enabled) return false;
            if (e.IsClick(Button)) {
                if (e.IsJustPressed()) {
                    _startDragCameraPosition = e.GetMousePosition();
                    if (DragSmoothingSpeed >= 0f) {
                        _camera2DSmoothingSpeed = Camera2D.PositionSmoothingSpeed;
                        Camera2D.PositionSmoothingSpeed = DragSmoothingSpeed;
                    }
                    OnDrag?.Invoke(true);
                } else {
                    _startDragCameraPosition = null;
                    if (DragSmoothingSpeed >= 0f) {
                        Camera2D.PositionSmoothingSpeed = _camera2DSmoothingSpeed;
                    }
                    OnDrag?.Invoke(false);
                }
                return true;
            } else if (_startDragCameraPosition.HasValue && e.IsMouseMotion()) {
                var newPosition = _startDragCameraPosition.Value - e.GetMousePosition();
                Camera2D.Position += newPosition * DragSensitivity;
                _startDragCameraPosition = e.GetMousePosition();
                return true;
            }
            return false;
        }
    }
}