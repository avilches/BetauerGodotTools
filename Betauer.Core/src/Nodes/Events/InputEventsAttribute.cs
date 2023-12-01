using System;

namespace Betauer.Core.Nodes.Events;

[AttributeUsage(AttributeTargets.Class)]
public class InputEventsAttribute : Attribute {
    public bool Handled { get; init; } = true;
    public bool Unhandled { get; init; } = true;
    public bool UnhandledKey { get; init; } = true;
    public bool Shortcut { get; init; } = true;
}