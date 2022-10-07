using System;

namespace Betauer.StateMachine.Sync {
    public class StateMachineSync<TStateKey, TTransitionKey> : 
        BaseStateMachineSync<TStateKey, TTransitionKey, IStateSync<TStateKey, TTransitionKey>> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public StateMachineSync(TStateKey initialState, string? name = null) : base(initialState, name) {
        }

        public StateBuilderSync<TStateKey, TTransitionKey> State(TStateKey stateKey) {
            return new StateBuilderSync<TStateKey, TTransitionKey>(stateKey, AddState);
        }
    }
}