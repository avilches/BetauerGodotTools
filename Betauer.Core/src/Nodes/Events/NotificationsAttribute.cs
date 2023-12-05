using System;

namespace Betauer.Core.Nodes.Events;

[AttributeUsage(AttributeTargets.Class)]
public class NotificationsAttribute : Attribute {
    public bool Process { get; init; } = true;
    public bool PhysicsProcess { get; init; } = true;
}