using System;
using Godot;

namespace Betauer.Core.Nodes.Events;

public interface IUnhandledInputEvent : INodeEvent {
    public event Action<InputEvent> OnUnhandledInput;
}