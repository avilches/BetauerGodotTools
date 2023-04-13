using System;

namespace Betauer.FSM.Sync;

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
            AwakeFunc,
            ExecuteFunc,
            SuspendFunc,
            ExitFunc
        );
    }

    public StateBuilderSync(TStateKey key, Action<IStateSync<TStateKey, TEventKey>> build) : base(key, build) {
    }
}