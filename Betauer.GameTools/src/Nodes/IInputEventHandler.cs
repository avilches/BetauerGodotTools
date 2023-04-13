using Godot;

namespace Betauer.Nodes;

public interface IInputEventHandler : IEventHandler {
    public void Handle(InputEvent inputEvent);
}