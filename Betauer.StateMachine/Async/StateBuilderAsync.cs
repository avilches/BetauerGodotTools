using System;

namespace Betauer.StateMachine.Async;

public class StateBuilderAsync<TStateKey, TEventKey> :
    BaseStateBuilderAsync<StateBuilderAsync<TStateKey, TEventKey>, TStateKey, TEventKey>
    where TStateKey : Enum
    where TEventKey : Enum {
    protected override IStateAsync<TStateKey, TEventKey> CreateState() {
        return new StateAsync<TStateKey, TEventKey>(
            Key,
            EventRules,
            Conditions?.ToArray(),
            EnterFunc,
            ExecuteFunc,
            ExitFunc,
            SuspendFunc,
            AwakeFunc);
    }

    public StateBuilderAsync(TStateKey key, Action<IStateAsync<TStateKey, TEventKey>> build) : base(key, build) {
    }
}