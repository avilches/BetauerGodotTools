using System;
using System.Collections.Generic;

namespace Betauer.StateMachine.Sync; 

public class StateSync<TStateKey, TEventKey> :
    BaseState<TStateKey, TEventKey>, IStateSync<TStateKey, TEventKey>
    where TStateKey : Enum 
    where TEventKey : Enum {
        
    private readonly Action? _enter;
    private readonly Action? _awake;
    private readonly Action? _execute;
    private readonly Action? _suspend;
    private readonly Action? _exit;

    public StateSync(
        TStateKey key,
        Dictionary<TEventKey, EventRule<TStateKey, TEventKey>>? eventRules,
        Condition<TStateKey, TEventKey>[]? conditions,
        Action? enter,
        Action? awake,
        Action? execute,
        Action? suspend,
        Action? exit
        ) : 
        base(key, eventRules, conditions) {
        _enter = enter;
        _awake = awake;
        _execute = execute;
        _suspend = suspend;
        _exit = exit;
    }

    public void Enter() {
        _enter?.Invoke();
    }

    public void Awake() {
        _awake?.Invoke();
    }

    public void Execute() {
        _execute?.Invoke();
    }

    public void Suspend() {
        _suspend?.Invoke();
    }

    public void Exit() {
        _exit?.Invoke();
    }
}