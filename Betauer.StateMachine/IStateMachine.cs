using System;

namespace Betauer.StateMachine; 

public interface IStateMachineEvents<TStateKey> {
    public string? Name { get; }
    public event Action<TransitionArgs<TStateKey>>? OnEnter;
    public event Action<TransitionArgs<TStateKey>>? OnAwake;
    public event Action<TransitionArgs<TStateKey>>? OnSuspend;
    public event Action<TransitionArgs<TStateKey>>? OnExit;
    public event Action<TransitionArgs<TStateKey>>? OnTransition;
    public event Action? OnBefore;
    public event Action? OnAfter;
}
    
public interface IStateMachine<TStateKey, TEventKey, TState> : IStateMachineEvents<TStateKey>
    where TStateKey : Enum where TEventKey : Enum {
        
    public void AddState(TState state);
    public void AddEventRule(TEventKey eventKey, EventRule<TStateKey, TEventKey> eventRule);
    public bool IsState(TStateKey state);
    public TState CurrentState { get; }
    public void Send(TEventKey eventKey, int weight);
    public void Reset();

}