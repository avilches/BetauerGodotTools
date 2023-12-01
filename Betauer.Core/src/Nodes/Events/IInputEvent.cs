using System;
using Godot;

namespace Betauer.Core.Nodes.Events;

public interface IInputEvent : INodeEvent {
    public event Action<InputEvent>? OnInput;
}