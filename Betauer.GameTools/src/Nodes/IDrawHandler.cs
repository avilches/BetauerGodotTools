using Godot;

namespace Betauer.Nodes;

public interface IDrawHandler : IEventHandler {
    public void Handle(CanvasItem canvas);
    public bool Redraw { get; set; }
}