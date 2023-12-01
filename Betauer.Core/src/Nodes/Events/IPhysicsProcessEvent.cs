using System;

namespace Betauer.Core.Nodes.Events;

public interface IPhysicsProcessEvent : INodeEvent {
    /// <summary>
    /// <para>Event called every frame when the physics process flag is set (see <see cref="M:Godot.Node.SetPhysicsProcess(System.Boolean)" />).</para>
    /// </summary>
    public event Action<double> OnPhysicsProcess;
}