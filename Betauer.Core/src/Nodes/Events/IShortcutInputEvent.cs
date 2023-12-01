using System;
using Godot;

namespace Betauer.Core.Nodes.Events;

public interface IShortcutInputEvent : INodeEvent {
    public event Action<InputEvent> OnShortcutInput;
}