using System;
using Godot;

namespace Betauer.Nodes;

public class InputEventEventHandler : BaseEventHandler, IInputEventHandler {
    private readonly Action<InputEvent> _delegate;
    public InputEventEventHandler(string? name, Action<InputEvent> @delegate, Node.ProcessModeEnum processMode) : base(name, processMode) {
        _delegate = @delegate;
    }

    public void Handle(InputEvent delta) {
        _delegate(delta);
    }
}