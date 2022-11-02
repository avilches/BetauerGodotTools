using System;

namespace Betauer.StateMachine.Sync {
    public class StateBuilderSync<TStateKey, TEventKey> : BaseStateBuilderSync<StateBuilderSync<TStateKey, TEventKey>, TStateKey, TEventKey> 
        where TStateKey : Enum where TEventKey : Enum {
        
        protected override IStateSync<TStateKey, TEventKey> CreateState() {
            return new StateSync<TStateKey, TEventKey>(Key, EnterFunc, Conditions?.ToArray(), ExecuteFunc, ExitFunc, SuspendFunc, AwakeFunc, Events);
        }

        public StateBuilderSync(TStateKey key, Action<IStateSync<TStateKey, TEventKey>> build) : base(key, build) {
        }
    }
}