using System;
using Betauer.StateMachine.Sync;

namespace Betauer.StateMachine {
    public interface IStateMachine<TStateKey, TEventKey, TState>
        where TStateKey : Enum where TEventKey : Enum {
        
        public void AddState(TState state);
        public void AddEvent(TEventKey eventKey, Event<TStateKey, TEventKey> @event);
        public bool IsState(TStateKey state);
        public TState CurrentState { get; }
        public void Enqueue(TEventKey name);
        public string? Name { get; }
        public void AddOnEnter(Action<TransitionArgs<TStateKey>> e);
        public void AddOnAwake(Action<TransitionArgs<TStateKey>> e);
        public void AddOnSuspend(Action<TransitionArgs<TStateKey>> e);
        public void AddOnExit(Action<TransitionArgs<TStateKey>> e);
        public void AddOnTransition(Action<TransitionArgs<TStateKey>> e);
        public void RemoveOnEnter(Action<TransitionArgs<TStateKey>> e);
        public void RemoveOnAwake(Action<TransitionArgs<TStateKey>> e);
        public void RemoveOnSuspend(Action<TransitionArgs<TStateKey>> e);
        public void RemoveOnExit(Action<TransitionArgs<TStateKey>> e);
        public void RemoveOnTransition(Action<TransitionArgs<TStateKey>> e);
    }
}