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
        Action? before,
        Action? enter,
        Action? awake,
        Action? execute,
        Action? suspend,
        Action? exit,
        Action? after,
        Action<InputEvent>? input,
        Action<InputEvent>? shortcutInput,
        Action<InputEvent>? unhandledInput,
        Action<InputEvent>? unhandledKeyInput
    ) : base(key,
        eventRules,
        conditions,
        before,
        enter,
        awake,
        execute,
        suspend,
        exit,
        after) {
        InputHandler = new StateNodeInputHandler(
            input,
            shortcutInput,
            unhandledInput,
            unhandledKeyInput
        );
    }
}