using System;

namespace Betauer.StateMachine.Async {
    public class StateMachineAsync<TStateKey, TTransitionKey> : 
        BaseStateMachineAsync<TStateKey, TTransitionKey, IStateAsync<TStateKey, TTransitionKey>> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public StateMachineAsync(TStateKey initialState, string? name = null) : base(initialState, name) {
        }

        public EventBuilder<StateMachineAsync<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> On(
            TTransitionKey transitionKey) {
            return On(this, transitionKey);
        }

        public StateBuilderAsync<TStateKey, TTransitionKey> State(TStateKey stateKey) {
            return new StateBuilderAsync<TStateKey, TTransitionKey>(stateKey, AddState);
        }
    }
}