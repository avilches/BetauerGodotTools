using System;

namespace Betauer.StateMachine.Sync;

public class StateBuilderSync<TStateKey, TEventKey> :
    BaseStateBuilderSync<StateBuilderSync<TStateKey, TEventKey>, TStateKey, TEventKey>
    where TStateKey : Enum
    where TEventKey : Enum {
    protected override IStateSync<TStateKey, TEventKey> CreateState() {
        return new StateSync<TStateKey, TEventKey>(
            Key,
            EventRules,
            Conditions?.ToArray(),
            EnterFunc,
            ExecuteFunc,
            ExitFunc,
            SuspendFunc,
            AwakeFunc);
    }

    public StateBuilderSync(TStateKey key, Action<IStateSync<TStateKey, TEventKey>> build) : base(key, build) {
    }
}