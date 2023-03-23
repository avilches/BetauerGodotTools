using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betauer.FSM.Async; 

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
        Dictionary<TEventKey, EventRule<TStateKey, TEventKey>>? eventRules,
        Condition<TStateKey, TEventKey>[]? conditions,
        Func<Task>? enter,
        Func<Task>? awake,
        Func<Task>? execute,
        Func<Task>? suspend,
        Func<Task>? exit
        ) :
        base(key, eventRules, conditions) {
        _enter = enter;
        _awake = awake;
        _execute = execute;
        _suspend = suspend;
        _exit = exit;
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