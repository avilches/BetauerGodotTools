using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.FSM.Sync;

public class StateNodeSync<TStateKey, TEventKey> : StateSync<TStateKey, TEventKey>
    where TEventKey : Enum where TStateKey : Enum {
    public readonly StateNodeInputHandler InputHandler;

    public StateNodeSync(TStateKey key,
        Dictionary<TEventKey, EventRule<TStateKey, TEventKey>>? eventRules,
        Condition<TStateKey, TEventKey>[]? conditions,
        Action? enter,
        Action? awake,
        Action? execute,
        Action? suspend,
        Action? exit,
        Action<InputEvent>? input,
        Action<InputEvent>? shortcutInput,
        Action<InputEvent>? unhandledInput,
        Action<InputEvent>? unhandledKeyInput
    ) : base(key,
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