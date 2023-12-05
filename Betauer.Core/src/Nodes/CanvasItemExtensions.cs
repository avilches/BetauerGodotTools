using System;
using Godot;

namespace Betauer.Core.Nodes;

public static partial class CanvasItemExtensions {
    public static Vector2 ToLocal(this CanvasItem canvasItem, Vector2 globalPosition) {
        return globalPosition - canvasItem.GetGlobalTransform().Origin;
    }

    /// <summary>
    /// The VisibilityChanged signal is always called when the visibility of the node changes no matter if the node was visible or invisible before.
    /// That means if the parent's node was not visible and the node.Visible is changed, the signal is triggered either way.
    ///
    /// The Hidden signal is only called when the node was visible and now is not visible, no matter if the change comes from the node or from a parent.
    ///
    /// This create a inconsistent behaviour between these two signals. This method is a workaround to have a consistent behaviour. 
    /// 
    /// </summary>
    /// <param name="canvasItem"></param>
    /// <param name="action"></param>
    public static void OnVisible(this CanvasItem canvasItem, Action<bool> action) {
        canvasItem.VisibilityChanged += () => action(canvasItem.IsVisibleInTree());
    }
}