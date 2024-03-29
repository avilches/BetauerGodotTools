using System;
using Godot;

namespace Betauer.Input.Controller;

public class DragAndDropController {
    public Vector2? LastDragPosition { get; private set; }

    public bool IsDragging => LastDragPosition.HasValue;
    public MouseButton MouseButton { get; set; } = MouseButton.Left;

    public Func<InputEvent, bool>? Predicate;
        
    /// <summary>
    /// Receive the e.GetMousePosition when the drag starts
    /// </summary>
    public event Action<Vector2>? OnStartDrag;
    /// <summary>
    /// Receive the offset since the last movement so you can add or subtract to your object (or camera) position
    /// </summary>
    public event Action<Vector2>? OnDrag;
    /// <summary>
    /// Receive the e.GetMousePosition when the drag ends (drop)
    /// </summary>
    public event Action<Vector2>? OnDrop;

    public DragAndDropController WithMouseButton(MouseButton mouseButton) {
        MouseButton = mouseButton;
        return this;
    }

    public DragAndDropController OnlyIf(Func<InputEvent, bool>? isClickPredicate) {
        Predicate = isClickPredicate;
        return this;
    }

    
    public void ForceDrop(Vector2? dropPosition = null) {
        if (LastDragPosition.HasValue) {
            var mousePosition = dropPosition ?? ((SceneTree)Engine.GetMainLoop()).Root.GetMousePosition();
            OnDrop?.Invoke(mousePosition);
            LastDragPosition = null;
        }
    }

    public void Handle(InputEvent e) {
        if (e.IsClick(MouseButton) && (Predicate == null || Predicate(e))) {
            if (e.IsPressed()) {
                LastDragPosition = e.GetMousePosition();
                OnStartDrag?.Invoke(LastDragPosition.Value);
            } else if (e.IsReleased()) {
                OnDrop?.Invoke(e.GetMousePosition());
                LastDragPosition = null;
            }
        } else if (LastDragPosition.HasValue && e.IsMouseMotion() && Godot.Input.IsMouseButtonPressed(MouseButton)) {
            var offset = e.GetMousePosition() - LastDragPosition.Value;
            OnDrag?.Invoke(offset);
            LastDragPosition = e.GetMousePosition();;
        }
    }
}