using System;
using Godot;

namespace Betauer.Nodes;

public class InputEventHandler : BaseHandler, IInputEventHandler {
    private readonly Action<InputEvent> _delegate;

    public InputEventHandler(Action<InputEvent> @delegate, string? name = null) : base(name) {
        _delegate = @delegate;
    }

    public void Handle(InputEvent delta) {
        _delegate(delta);
    }
}