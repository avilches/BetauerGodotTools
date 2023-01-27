using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.StateMachine.Sync;

public class StateNodeSync<TStateKey, TEventKey> : StateSync<TStateKey, TEventKey>
    where TEventKey : Enum where TStateKey : Enum {
    public readonly StateNodeInputHandler InputHandler;

    public StateNodeSync(TStateKey key,
        Dictionary<TEventKey, EventRule<TStateKey, TEventKey>>? eventRules,
        Condition<TStateKey, TEventKey>[]? conditions,
        Action? enter,
        Action? execute,
        Action? exit,
        Action? suspend,
        Action? awake,
        Action<InputEvent>? input,
        Action<InputEvent>? shortcutInput,
        Action<InputEvent>? unhandledInput,
        Action<InputEvent>? unhandledKeyInput
    ) : base(key, eventRules, conditions, enter, execute, exit, suspend, awake) {
        InputHandler = new StateNodeInputHandler(
            input, 
            shortcutInput, 
            unhandledInput, 
            unhandledKeyInput 
        );
    }
}