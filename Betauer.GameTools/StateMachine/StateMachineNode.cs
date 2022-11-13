using System;
using Godot;

namespace Betauer.StateMachine {
    public abstract partial class StateMachineNode<TStateKey> : Node 
        where TStateKey : Enum {
        
        private event Action<double, TStateKey>? OnExecuteStart;
        private event Action<TStateKey>? OnExecuteEnd;
        public void AddOnExecuteStart(Action<double, TStateKey> e) => OnExecuteStart += e;
        public void AddOnExecuteEnd(Action<TStateKey> e) => OnExecuteEnd += e;
        public void RemoveOnExecuteStart(Action<double, TStateKey> e) => OnExecuteStart -= e;
        public void RemoveOnExecuteEnd(Action<TStateKey> e) => OnExecuteEnd -= e;
        public bool ProcessInPhysics { get; set; }

        protected void ExecuteStart(double delta, TStateKey state) => OnExecuteStart?.Invoke(delta, state);
        protected void ExecuteEnd(TStateKey state) => OnExecuteEnd?.Invoke(state);
    }
}