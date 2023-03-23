using System;

namespace Betauer.FSM;

public interface IFsm<TStateKey, TEventKey, TState> : IFsmEvents<TStateKey>
    where TStateKey : Enum where TEventKey : Enum {
        
    public void AddState(TState state);
    public void AddEventRule(TEventKey eventKey, EventRule<TStateKey, TEventKey> eventRule);
    public bool IsState(TStateKey state);
    public TState CurrentState { get; }
    public void Send(TEventKey eventKey, int weight);
    public void Reset();

}