using System;
using Betauer.Core.Nodes;
using Godot;

namespace Betauer.Nodes;

public static partial class CanvasItemExtensions {
    public static Drawer AddDraw(this CanvasItem canvasItem, Action<CanvasItem> action) {
        var drawer = canvasItem as Drawer ?? GetDrawer(canvasItem);
        drawer.OnDraw += action;
        return drawer;
    }

    public static Drawer RemoveDraw(this CanvasItem canvasItem, Action<CanvasItem> action) {
        var drawer = canvasItem as Drawer ?? GetDrawer(canvasItem);
        drawer.OnDraw -= action;
        return drawer;
    }

    private static Drawer GetDrawer(this CanvasItem canvasItem) {
        var drawer = canvasItem.FirstNode<Drawer>();
        if (drawer != null) return drawer;
        drawer = new Drawer {
            Name = "Drawer"
        };
        canvasItem.AddChild(drawer);
        return drawer;
    }
}