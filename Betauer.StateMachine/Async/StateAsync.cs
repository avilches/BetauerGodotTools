using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async; 

public class StateAsync<TStateKey, TEventKey> : 
    BaseState<TStateKey, TEventKey>, IStateAsync<TStateKey, TEventKey>
    where TStateKey : Enum
    where TEventKey : Enum {
        
    private readonly Func<Task>? _enter;
    private readonly Func<Task>? _awake;
    private readonly Func<Task>? _execute;
    private readonly Func<Task>? _suspend;
    private readonly Func<Task>? _exit;

    public StateAsync(
        TStateKey key,
        Func<Task>? enter,
        Condition<TStateKey, TEventKey>[] conditions,
        Func<Task>? execute,
        Func<Task>? exit,
        Func<Task>? suspend,
        Func<Task>? awake,
        Dictionary<TEventKey, Event<TStateKey, TEventKey>>? events) : base(key, events, conditions) {

        _enter = enter;
        _execute = execute;
        _exit = exit;
        _suspend = suspend;
        _awake = awake;
    }

    public Task Enter() {
        return _enter != null ? _enter.Invoke() : Task.CompletedTask;
    }

    public Task Awake() {
        return _awake != null ? _awake.Invoke() : Task.CompletedTask;
    }

    public Task Execute() {
        return _execute != null ? _execute.Invoke() : Task.CompletedTask;
    }

    public Task Suspend() {
        return _suspend != null ? _suspend.Invoke() : Task.CompletedTask;
    }

    public Task Exit() {
        return _exit != null ? _exit.Invoke() : Task.CompletedTask;
    }
}