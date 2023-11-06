using Godot;

namespace Betauer.Core.Nodes;

public static partial class CanvasItemExtensions {
    public static Vector2 ToLocal(this CanvasItem canvasItem, Vector2 globalPosition) {
        return globalPosition - canvasItem.GetGlobalTransform().Origin;
    }
}