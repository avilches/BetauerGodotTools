using System;
using Godot;

namespace Betauer.Nodes;

public class DrawEventHandler : BaseEventHandler, IDrawHandler {
    private readonly Action<CanvasItem> _delegate;

    public DrawEventHandler(string? name, Action<CanvasItem> @delegate, Node.ProcessModeEnum processMode) : base(name, processMode) {
        _delegate = @delegate;
    }

    public void Handle(CanvasItem canvas) {
        _delegate(canvas);
    }
}