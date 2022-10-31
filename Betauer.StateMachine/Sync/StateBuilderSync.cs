using System;

namespace Betauer.StateMachine.Sync {
    public class StateBuilderSync<TStateKey, TTransitionKey> : BaseStateBuilderSync<StateBuilderSync<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        protected override IStateSync<TStateKey, TTransitionKey> CreateState() {
            return new StateSync<TStateKey, TTransitionKey>(Key, EnterFunc, Conditions?.ToArray(), ExecuteFunc, ExitFunc, SuspendFunc, AwakeFunc, Events);
        }

        public StateBuilderSync(TStateKey key, Action<IStateSync<TStateKey, TTransitionKey>> build) : base(key, build) {
        }
    }
}