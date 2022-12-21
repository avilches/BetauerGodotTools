using System;

namespace Betauer.StateMachine {
    public interface IStateMachine<TStateKey, TEventKey, TState>
        where TStateKey : Enum where TEventKey : Enum {
        
        public void AddState(TState state);
        public void AddEvent(TEventKey eventKey, Event<TStateKey, TEventKey> @event);
        public bool IsState(TStateKey state);
        public TState CurrentState { get; }
        public void Enqueue(TEventKey name);
        public string? Name { get; }
        public event Action<TransitionArgs<TStateKey>>? OnEnter;
        public event Action<TransitionArgs<TStateKey>>? OnAwake;
        public event Action<TransitionArgs<TStateKey>>? OnSuspend;
        public event Action<TransitionArgs<TStateKey>>? OnExit;
        public event Action<TransitionArgs<TStateKey>>? OnTransition;

    }
}