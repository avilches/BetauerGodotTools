using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.StateMachine.Sync; 

public class StateNodeSync<TStateKey, TEventKey> : StateSync<TStateKey, TEventKey> 
    where TEventKey : Enum where TStateKey : Enum {
        
    private readonly Action<InputEvent>? _input;
    private readonly Action<InputEvent>? _unhandledInput;

    public StateNodeSync(TStateKey key, 
        Action? enter,
        Condition<TStateKey, TEventKey>[] conditions,
        Action execute,
        Action? exit, 
        Action? suspend, 
        Action? awake,
        Dictionary<TEventKey, Event<TStateKey, TEventKey>>? events,
        Action<InputEvent> input, 
        Action<InputEvent> unhandledInput) : base(key, enter, conditions, execute, exit, suspend, awake, events) {
        _input = input;
        _unhandledInput = unhandledInput;
    }

    public void _Input(InputEvent e) => _input?.Invoke(e);
    public void _UnhandledInput(InputEvent e) => _unhandledInput?.Invoke(e);
}