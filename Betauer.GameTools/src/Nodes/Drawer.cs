using System;
using Godot;

namespace Betauer.Nodes;

public partial class Drawer : Node2D {
    public event Action<CanvasItem>? OnDraw;

    public override void _Draw() {
        OnDraw?.Invoke(this);
    }
    
}