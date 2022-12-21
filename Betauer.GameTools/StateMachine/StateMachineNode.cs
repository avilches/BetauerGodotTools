using System;
using Godot;

namespace Betauer.StateMachine {
    public abstract partial class StateMachineNode<TStateKey> : Node 
        where TStateKey : Enum {
        
        public event Action<double>? OnExecuteStart;
        public event Action? OnExecuteEnd;
        public bool ProcessInPhysics { get; set; }

        protected void ExecuteStart(double delta) => OnExecuteStart?.Invoke(delta);
        protected void ExecuteEnd() => OnExecuteEnd?.Invoke();
    }
}