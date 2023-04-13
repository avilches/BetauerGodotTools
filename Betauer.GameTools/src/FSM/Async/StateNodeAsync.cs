using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Betauer.FSM.Async;

public class StateNodeAsync<TStateKey, TEventKey> : StateAsync<TStateKey, TEventKey>
    where TEventKey : Enum where TStateKey : Enum {
    public readonly StateNodeInputHandler InputHandler;

    public StateNodeAsync(TStateKey key,
        Dictionary<TEventKey, EventRule<TStateKey, TEventKey>>? eventRules,
        Condition<TStateKey, TEventKey>[]? conditions,
        Func<Task>? enter,
        Func<Task>? awake,
        Func<Task>? execute,
        Func<Task>? suspend,
        Func<Task>? exit,
        Action<InputEvent>? input,
        Action<InputEvent>? shortcutInput,
        Action<InputEvent>? unhandledInput,
        Action<InputEvent>? unhandledKeyInput
    ) : base(
        key,
        eventRules,
        conditions,
        enter,
        awake,
        execute,
        suspend,
        exit) {
        InputHandler = new StateNodeInputHandler(
            input,
            shortcutInput,
            unhandledInput,
            unhandledKeyInput
        );
    }
}