using System;
using Godot;

namespace Betauer.Nodes;

public class InputEventNodeEventHandler : BaseNodeEventHandler, IInputEventHandler {
    private readonly Action<InputEvent> _delegate;
    public InputEventNodeEventHandler(Node? node, Action<InputEvent> @delegate, Node.ProcessModeEnum processMode) : base(node, processMode) {
        _delegate = @delegate;
    }

    public void Handle(InputEvent delta) {
        _delegate(delta);
    }
}