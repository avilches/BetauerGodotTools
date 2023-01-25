using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Betauer.StateMachine.Async;

public class StateNodeAsync<TStateKey, TEventKey> : StateAsync<TStateKey, TEventKey>
    where TEventKey : Enum where TStateKey : Enum {
    public readonly StateNodeInputHandler InputHandler;

    public StateNodeAsync(TStateKey key,
        Func<Task>? enter,
        Condition<TStateKey, TEventKey>[] conditions,
        Func<Task> execute,
        Func<Task>? exit,
        Func<Task>? suspend,
        Func<Task>? awake,
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