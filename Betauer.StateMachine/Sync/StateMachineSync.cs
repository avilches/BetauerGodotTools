using System;

namespace Betauer.StateMachine.Sync {
    public class StateMachineSync<TStateKey, TEventKey> : 
        BaseStateMachineSync<TStateKey, TEventKey, IStateSync<TStateKey, TEventKey>> 
        where TStateKey : Enum where TEventKey : Enum {
        
        public StateMachineSync(TStateKey initialState, string? name = null) : base(initialState, name) {
        }

        public EventBuilder<StateMachineSync<TStateKey, TEventKey>, TStateKey, TEventKey> On(
            TEventKey eventKey) {
            return On(this, eventKey);
        }

        public StateBuilderSync<TStateKey, TEventKey> State(TStateKey stateKey) {
            return new StateBuilderSync<TStateKey, TEventKey>(stateKey, AddState);
        }
    }
}