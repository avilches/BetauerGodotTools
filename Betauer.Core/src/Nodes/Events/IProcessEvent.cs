using System;

namespace Betauer.Core.Nodes.Events;

public interface IProcessEvent : INodeEvent {
    /// <summary>
    /// <para>Event called every frame when the process flag is set (see <see cref="M:Godot.Node.SetProcess(System.Boolean)" />).</para>
    /// </summary>
    public event Action<double> OnProcess;
}