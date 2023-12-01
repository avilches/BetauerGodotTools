using System;

namespace Betauer.Core.Nodes.Events;

[AttributeUsage(AttributeTargets.Class)]
public class NotificationAttribute : Attribute {
    public bool Process { get; init; } = true;
    public bool PhysicsProcess { get; init; } = true;
}