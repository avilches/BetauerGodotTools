using System;
using System.Threading.Tasks;

namespace Betauer.FSM.Async;

public abstract class BaseStateBuilderAsync<TBuilder, TStateKey, TEventKey> :
    BaseStateBuilder<TBuilder, TStateKey, TEventKey>
    where TStateKey : Enum
    where TEventKey : Enum
    where TBuilder : class {
    
    protected Func<Task>? EnterFunc;
    protected Func<Task>? AwakeFunc;
    protected Func<Task>? ExecuteFunc;
    protected Func<Task>? ExitFunc;
    protected Func<Task>? SuspendFunc;
    protected readonly Action<IStateAsync<TStateKey, TEventKey>> OnBuild;

    protected abstract IStateAsync<TStateKey, TEventKey> CreateState();

    protected BaseStateBuilderAsync(TStateKey key, Action<IStateAsync<TStateKey, TEventKey>> build) : base(key) {
        OnBuild = build;
    }

    public IStateAsync<TStateKey, TEventKey> Build() {
        IStateAsync<TStateKey, TEventKey> state = CreateState();
        OnBuild(state);
        return state;
    }

    /*
     * Enter
     */
    public TBuilder Enter(Func<Task> enter) {
        EnterFunc = enter;
        return this as TBuilder;
    }

    public TBuilder Enter(Action enter) {
        EnterFunc = () => {
            enter();
            return Task.CompletedTask;
        };
        return this as TBuilder;
    }

    /*
     * Awake
     */
    public TBuilder Awake(Func<Task> awake) {
        AwakeFunc = awake;
        return this as TBuilder;
    }

    public TBuilder Awake(Action awake) {
        AwakeFunc = () => {
            awake();
            return Task.CompletedTask;
        };
        return this as TBuilder;
    }

    /*
     * Execute
     */
    public TBuilder Execute(Func<Task> execute) {
        ExecuteFunc = execute;
        return this as TBuilder;
    }

    public TBuilder Execute(Action execute) {
        ExecuteFunc = () => {
            execute();
            return Task.CompletedTask;
        };
        return this as TBuilder;
    }

    /*
     * Suspend
     */
    public TBuilder Suspend(Func<Task> suspend) {
        SuspendFunc = suspend;
        return this as TBuilder;
    }

    public TBuilder Suspend(Action suspend) {
        SuspendFunc = () => {
            suspend();
            return Task.CompletedTask;
        };
        return this as TBuilder;
    }

    /*
     * Exit
     */
    public TBuilder Exit(Func<Task> exit) {
        ExitFunc = exit;
        return this as TBuilder;
    }

    public TBuilder Exit(Action exit) {
        ExitFunc = () => {
            exit();
            return Task.CompletedTask;
        };
        return this as TBuilder;
    }
}