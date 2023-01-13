using System;

namespace Betauer.StateMachine {
    public interface IStateMachineEvents<TStateKey> {
        public string? Name { get; }
        public event Action<TransitionArgs<TStateKey>>? OnEnter;
        public event Action<TransitionArgs<TStateKey>>? OnAwake;
        public event Action<TransitionArgs<TStateKey>>? OnSuspend;
        public event Action<TransitionArgs<TStateKey>>? OnExit;
        public event Action<TransitionArgs<TStateKey>>? OnTransition;
        public event Action? OnBeforeExecute;
        public event Action? OnAfterExecute;
    }
    
    public interface IStateMachine<TStateKey, TEventKey, TState> : IStateMachineEvents<TStateKey>
        where TStateKey : Enum where TEventKey : Enum {
        
        public void AddState(TState state);
        public void AddEvent(TEventKey eventKey, Event<TStateKey, TEventKey> @event);
        public bool IsState(TStateKey state);
        public TState CurrentState { get; }
        public void Enqueue(TEventKey name);

    }
}