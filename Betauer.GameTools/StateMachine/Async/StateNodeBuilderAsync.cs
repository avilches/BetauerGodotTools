using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.StateMachine.Async;

public class StateNodeBuilderAsync<TStateKey, TEventKey> :
    BaseStateBuilderAsync<StateNodeBuilderAsync<TStateKey, TEventKey>, TStateKey, TEventKey>
    where TStateKey : Enum where TEventKey : Enum {
    public StateNodeBuilderAsync(TStateKey key, Action<IStateAsync<TStateKey, TEventKey>> build) : base(key, build) {
    }

    protected override IStateAsync<TStateKey, TEventKey> CreateState() {
        return new StateNodeAsync<TStateKey, TEventKey>(Key,
            EnterFunc,
            Conditions.ToArray(),
            ExecuteFunc,
            ExitFunc,
            SuspendFunc,
            AwakeFunc,
            Events,
            Input,
            ShortcutInput,
            UnhandledInput,
            UnhandledKeyInput,
            InputBatch,
            ShortcutInputBatch,
            UnhandledInputBatch,
            UnhandledInputKeyBatch);
    }

    private event Action<InputEvent>? Input;
    private event Action<InputEvent>? ShortcutInput;
    private event Action<InputEvent>? UnhandledInput;
    private event Action<InputEvent>? UnhandledKeyInput;
    private event Action<IEnumerable<InputEvent>>? InputBatch;
    private event Action<IEnumerable<InputEvent>>? ShortcutInputBatch;
    private event Action<IEnumerable<InputEvent>>? UnhandledInputBatch;
    private event Action<IEnumerable<InputEvent>>? UnhandledInputKeyBatch;


    public StateNodeBuilderAsync<TStateKey, TEventKey> OnInput(Action<InputEvent> input) {
        Input += input;
        return this;
    }

    public StateNodeBuilderAsync<TStateKey, TEventKey> OnShortcutInput(Action<InputEvent> shortcutInput) {
        ShortcutInput += shortcutInput;
        return this;
    }

    public StateNodeBuilderAsync<TStateKey, TEventKey> OnUnhandledInput(Action<InputEvent> unhandledInput) {
        UnhandledInput += unhandledInput;
        return this;
    }

    public StateNodeBuilderAsync<TStateKey, TEventKey> OnUnhandledKeyInput(Action<InputEvent> unhandledKeyInput) {
        UnhandledKeyInput += unhandledKeyInput;
        return this;
    }

    public StateNodeBuilderAsync<TStateKey, TEventKey> OnInputBatch(Action<IEnumerable<InputEvent>> inputBatch) {
        InputBatch += inputBatch;
        return this;
    }

    public StateNodeBuilderAsync<TStateKey, TEventKey> OnShortcutInputBatch(Action<IEnumerable<InputEvent>> shortcutInputBatch) {
        ShortcutInputBatch += shortcutInputBatch;
        return this;
    }

    public StateNodeBuilderAsync<TStateKey, TEventKey> OnUnhandledInputBatch(Action<IEnumerable<InputEvent>> unhandledInputBatch) {
        UnhandledInputBatch += unhandledInputBatch;
        return this;
    }

    public StateNodeBuilderAsync<TStateKey, TEventKey> OnUnhandledInputKeyBatch(Action<IEnumerable<InputEvent>> unhandledInputKeyBatch) {
        UnhandledInputKeyBatch += unhandledInputKeyBatch;
        return this;
    }
}