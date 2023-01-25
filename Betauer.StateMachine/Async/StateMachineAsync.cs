using System;

namespace Betauer.StateMachine.Async; 

public class StateMachineAsync<TStateKey, TEventKey> : 
    BaseStateMachineAsync<TStateKey, TEventKey, IStateAsync<TStateKey, TEventKey>> 
    where TStateKey : Enum
    where TEventKey : Enum {
        
    public StateMachineAsync(TStateKey initialState, string? name = null) : base(initialState, name) {
    }

    public EventBuilder<StateMachineAsync<TStateKey, TEventKey>, TStateKey, TEventKey> On(
        TEventKey eventKey) {
        return On(this, eventKey);
    }

    public StateBuilderAsync<TStateKey, TEventKey> State(TStateKey stateKey) {
        return new StateBuilderAsync<TStateKey, TEventKey>(stateKey, AddState);
    }
}