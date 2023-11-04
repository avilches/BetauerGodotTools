using System;
using Godot;

namespace Betauer.Nodes;

public class DrawHandler : BaseHandler, IDrawHandler {
    private readonly Action<CanvasItem> _delegate;

    public bool Redraw { get; set; } = true;

    public DrawHandler(Action<CanvasItem> @delegate, Node.ProcessModeEnum processMode, string? name = null) : base(processMode, name) {
        _delegate = @delegate;
    }

    public void Handle(CanvasItem canvas) {
        _delegate(canvas);
    }
}