using System;

namespace Betauer.FSM.Async; 

public class FsmAsync<TStateKey, TEventKey> : 
    BaseFsmAsync<TStateKey, TEventKey, IStateAsync<TStateKey, TEventKey>> 
    where TStateKey : Enum
    where TEventKey : Enum {
        
    public FsmAsync(TStateKey initialState, string? name = null) : base(initialState, name) {
    }

    public EventBuilder<FsmAsync<TStateKey, TEventKey>, TStateKey, TEventKey> On(
        TEventKey eventKey) {
        return On(this, eventKey);
    }

    public StateBuilderAsync<TStateKey, TEventKey> State(TStateKey stateKey) {
        return new StateBuilderAsync<TStateKey, TEventKey>(stateKey, AddState);
    }
}