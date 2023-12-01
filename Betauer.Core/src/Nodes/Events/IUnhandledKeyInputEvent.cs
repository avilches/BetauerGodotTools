using System;
using Godot;

namespace Betauer.Core.Nodes.Events;

public interface IUnhandledKeyInputEvent : INodeEvent {
    public event Action<InputEvent> OnUnhandledKeyInput;
}