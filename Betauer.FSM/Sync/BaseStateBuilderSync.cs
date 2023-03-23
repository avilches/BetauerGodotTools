using System;

namespace Betauer.FSM.Sync; 

public abstract class BaseStateBuilderSync<TBuilder, TStateKey, TEventKey> :
    BaseStateBuilder<TBuilder, TStateKey, TEventKey>
    where TStateKey : Enum
    where TEventKey : Enum
    where TBuilder : class {
        
    protected Action? EnterFunc;
    protected Action? AwakeFunc;
    protected Action? ExecuteFunc;
    protected Action? SuspendFunc;
    protected Action? ExitFunc;
    protected readonly Action<IStateSync<TStateKey, TEventKey>> OnBuild;

    protected abstract IStateSync<TStateKey, TEventKey> CreateState();

    protected BaseStateBuilderSync(TStateKey key, Action<IStateSync<TStateKey, TEventKey>> build): base(key) {
        OnBuild = build;
    }

    public IStateSync<TStateKey, TEventKey> Build() {
        IStateSync<TStateKey, TEventKey> stateSync = CreateState();
        OnBuild(stateSync);
        return stateSync;
    }

    public TBuilder Enter(Action enter) {
        EnterFunc = enter;
        return this as TBuilder;
    }

    public TBuilder Awake(Action awake) {
        AwakeFunc = awake;
        return this as TBuilder;
    }

    public TBuilder Execute(Action execute) {
        ExecuteFunc = execute;
        return this as TBuilder;
    }

    public TBuilder Suspend(Action suspend) {
        SuspendFunc = suspend;
        return this as TBuilder;
    }

    public TBuilder Exit(Action exit) {
        ExitFunc = exit;
        return this as TBuilder;
    }

}