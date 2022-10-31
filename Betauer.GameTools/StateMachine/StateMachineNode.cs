using System;
using Godot;

namespace Betauer.StateMachine {
    public abstract class StateMachineNode<TStateKey> : Node 
        where TStateKey : Enum {
        
        private event Action<float, TStateKey>? OnExecuteStart;
        private event Action<TStateKey>? OnExecuteEnd;
        public void AddOnExecuteStart(Action<float, TStateKey> e) => OnExecuteStart += e;
        public void AddOnExecuteEnd(Action<TStateKey> e) => OnExecuteEnd += e;
        public void RemoveOnExecuteStart(Action<float, TStateKey> e) => OnExecuteStart -= e;
        public void RemoveOnExecuteEnd(Action<TStateKey> e) => OnExecuteEnd -= e;
        public ProcessMode Mode { get; set; }

        protected void ExecuteStart(float delta, TStateKey state) => OnExecuteStart?.Invoke(delta, state);
        protected void ExecuteEnd(TStateKey state) => OnExecuteEnd?.Invoke(state);
    }
}