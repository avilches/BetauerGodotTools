using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.StateMachine.Sync;

public class StateNodeSync<TStateKey, TEventKey> : StateSync<TStateKey, TEventKey>
    where TEventKey : Enum where TStateKey : Enum {
    public readonly StateNodeInputHandler InputHandler;

    public StateNodeSync(TStateKey key,
        Action? enter,
        Condition<TStateKey, TEventKey>[] conditions,
        Action? execute,
        Action? exit,
        Action? suspend,
        Action? awake,
        Dictionary<TEventKey, Event<TStateKey, TEventKey>>? events,
        Action<InputEvent>? input,
        Action<InputEvent>? shortcutInput,
        Action<InputEvent>? unhandledInput,
        Action<InputEvent>? unhandledKeyInput,
        Action<IEnumerable<InputEvent>>? inputBatch,
        Action<IEnumerable<InputEvent>>? shortcutInputBatch,
        Action<IEnumerable<InputEvent>>? unhandledInputBatch,
        Action<IEnumerable<InputEvent>>? unhandledInputKeyBatch
    ) : base(key, enter, conditions, execute, exit, suspend, awake, events) {
        InputHandler = new StateNodeInputHandler(
            input, 
            shortcutInput, 
            unhandledInput, 
            unhandledKeyInput, 
            inputBatch,
            shortcutInputBatch, 
            unhandledInputBatch, 
            unhandledInputKeyBatch
        );
    }
}