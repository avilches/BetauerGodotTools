using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async; 

public class StateAsync<TStateKey, TEventKey> : 
    BaseState<TStateKey, TEventKey>, IStateAsync<TStateKey, TEventKey>
    where TStateKey : Enum
    where TEventKey : Enum {
        
    private readonly Func<Task>? _before;
    private readonly Func<Task>? _enter;
    private readonly Func<Task>? _awake;
    private readonly Func<Task>? _execute;
    private readonly Func<Task>? _suspend;
    private readonly Func<Task>? _exit;
    private readonly Func<Task>? _after;

    public StateAsync(
        TStateKey key,
        Dictionary<TEventKey, EventRule<TStateKey, TEventKey>>? eventRules,
        Condition<TStateKey, TEventKey>[]? conditions,
        Func<Task>? before,
        Func<Task>? enter,
        Func<Task>? awake,
        Func<Task>? execute,
        Func<Task>? suspend,
        Func<Task>? exit,
        Func<Task>? after
        ) :
        base(key, eventRules, conditions) {
        _before = before;
        _enter = enter;
        _awake = awake;
        _execute = execute;
        _suspend = suspend;
        _exit = exit;
        _after = after;
    }

    public Task Before() {
        return _before != null ? _before.Invoke() : Task.CompletedTask;
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
    
    public Task After() {
        return _after != null ? _after.Invoke() : Task.CompletedTask;
    }
}