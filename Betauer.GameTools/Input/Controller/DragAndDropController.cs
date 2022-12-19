using System;
using Godot;

namespace Betauer.Input.Controller;

public class DragAndDropController {
    public Vector2? StartDragPosition { get; private set; }

    public bool IsDragging => StartDragPosition.HasValue;
    public InputAction Action { get; set; }

    public Func<InputEvent, bool>? Predicate;
        
    public event Action<Vector2>? OnStartDrag;
    public event Action<Vector2>? OnDrag;
    public event Action<Vector2>? OnDrop;

    public DragAndDropController(InputAction action) {
        Action = action;
    }

    public DragAndDropController OnlyIf(Func<InputEvent, bool>? isClickPredicate) {
        Predicate = isClickPredicate;
        return this;
    }

    public DragAndDropController AddOnStartDrag(Action<Vector2> e) {
        OnStartDrag += e;
        return this;
    }
        
    public DragAndDropController RemoveOnStartDrag(Action<Vector2> e) {
        OnStartDrag -= e;
        return this;
    }
        
    public DragAndDropController AddOnDrag(Action<Vector2> e) {
        OnDrag += e;
        return this;
    }
        
    public DragAndDropController RemoveOnDrag(Action<Vector2> e) {
        OnDrag -= e;
        return this;
    }
        
    public DragAndDropController AddOnDrop(Action<Vector2> e) {
        OnDrop += e;
        return this;
    }
        
    public DragAndDropController RemoveOnDrop(Action<Vector2> e) {
        OnDrop -= e;
        return this;
    }

    public void Handle(InputEvent e) {
        if (Action.IsEvent(e) && (Predicate == null || Predicate(e))) {
            if (e.IsJustPressed()) {
                StartDragPosition = e.GetMousePosition();
                OnStartDrag?.Invoke(StartDragPosition.Value);
            } else if (e.IsReleased()) {
                OnDrop?.Invoke(e.GetMousePosition());
                StartDragPosition = null;
            }
        } else if (StartDragPosition.HasValue && e.IsMouseMotion()) {
            var mouseMovement = StartDragPosition.Value - e.GetMousePosition();
            OnDrag?.Invoke(mouseMovement);
            StartDragPosition = e.GetMousePosition();
        }
    }
}