using System;

namespace Betauer.StateMachine.Async {
    public class StateBuilderAsync<TStateKey, TTransitionKey> : BaseStateBuilderAsync<StateBuilderAsync<TStateKey, TTransitionKey>, TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        
        protected override IStateAsync<TStateKey, TTransitionKey> CreateState() {
            return new StateAsync<TStateKey, TTransitionKey>(Key, EnterFunc, ExecuteFunc, ExitFunc, SuspendFunc, AwakeFunc, Events);
        }

        public StateBuilderAsync(TStateKey key, Action<IStateAsync<TStateKey, TTransitionKey>> build) : base(key, build) {
        }
    }
}