using System;
using Godot;

namespace Betauer.Nodes;

public class DrawNodeEventHandler : BaseNodeEventHandler, IDrawHandler {
    private readonly Action<CanvasItem> _delegate;
    public DrawNodeEventHandler(Node? node, Action<CanvasItem> @delegate, Node.ProcessModeEnum processMode) : base(node, processMode) {
        _delegate = @delegate;
    }

    public void Handle(CanvasItem canvas) {
        _delegate(canvas);
    }
}