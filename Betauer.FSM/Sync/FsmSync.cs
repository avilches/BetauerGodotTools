using System;

namespace Betauer.FSM.Sync; 

public class FsmSync<TStateKey, TEventKey> : 
    BaseFsmSync<TStateKey, TEventKey, IStateSync<TStateKey, TEventKey>> 
    where TStateKey : Enum
    where TEventKey : Enum {
        
    public FsmSync(TStateKey initialState, string? name = null) : base(initialState, name) {
    }

    public EventBuilder<FsmSync<TStateKey, TEventKey>, TStateKey, TEventKey> On(
        TEventKey eventKey) {
        return On(this, eventKey);
    }

    public StateBuilderSync<TStateKey, TEventKey> State(TStateKey stateKey) {
        return new StateBuilderSync<TStateKey, TEventKey>(stateKey, AddState);
    }
}