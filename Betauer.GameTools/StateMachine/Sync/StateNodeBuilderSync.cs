using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.StateMachine.Sync;

public class StateNodeBuilderSync<TStateKey, TEventKey> :
    BaseStateBuilderSync<StateNodeBuilderSync<TStateKey, TEventKey>, TStateKey, TEventKey>
    where TStateKey : Enum where TEventKey : Enum {
    public StateNodeBuilderSync(TStateKey key, Action<IStateSync<TStateKey, TEventKey>> build) : base(key, build) {
    }

    protected override IStateSync<TStateKey, TEventKey> CreateState() {
        return new StateNodeSync<TStateKey, TEventKey>(Key,
            EnterFunc,
            Conditions.ToArray(),
            ExecuteFunc,
            ExitFunc,
            SuspendFunc,
            AwakeFunc,
            Events,
            OnInputEvent,
            OnShortcutInputEvent,
            OnUnhandledInputEvent,
            OnUnhandledKeyInputEvent,
            OnInputBatchEvent,
            OnShortcutInputBatchEvent,
            OnUnhandledInputBatchEvent,
            OnUnhandledInputKeyBatchEvent);
    }

    private event Action<InputEvent>? OnInputEvent;
    private event Action<InputEvent>? OnShortcutInputEvent;
    private event Action<InputEvent>? OnUnhandledInputEvent;
    private event Action<InputEvent>? OnUnhandledKeyInputEvent;
    private event Action<IEnumerable<InputEvent>>? OnInputBatchEvent;
    private event Action<IEnumerable<InputEvent>>? OnShortcutInputBatchEvent;
    private event Action<IEnumerable<InputEvent>>? OnUnhandledInputBatchEvent;
    private event Action<IEnumerable<InputEvent>>? OnUnhandledInputKeyBatchEvent;
    
    public StateNodeBuilderSync<TStateKey, TEventKey> OnInput(Action<InputEvent> input) {
        OnInputEvent += input;
        return this;
    }

    public StateNodeBuilderSync<TStateKey, TEventKey> OnShortcutInput(Action<InputEvent> shortcutInput) {
        OnShortcutInputEvent += shortcutInput;
        return this;
    }

    public StateNodeBuilderSync<TStateKey, TEventKey> OnUnhandledInput(Action<InputEvent> unhandledInput) {
        OnUnhandledInputEvent += unhandledInput;
        return this;
    }

    public StateNodeBuilderSync<TStateKey, TEventKey> OnUnhandledKeyInput(Action<InputEvent> unhandledKeyInput) {
        OnUnhandledKeyInputEvent += unhandledKeyInput;
        return this;
    }

    public StateNodeBuilderSync<TStateKey, TEventKey> OnInputBatch(Action<IEnumerable<InputEvent>> inputBatch) {
        OnInputBatchEvent += inputBatch;
        return this;
    }

    public StateNodeBuilderSync<TStateKey, TEventKey> OnShortcutInputBatch(Action<IEnumerable<InputEvent>> shortcutInputBatch) {
        OnShortcutInputBatchEvent += shortcutInputBatch;
        return this;
    }

    public StateNodeBuilderSync<TStateKey, TEventKey> OnUnhandledInputBatch(Action<IEnumerable<InputEvent>> unhandledInputBatch) {
        OnUnhandledInputBatchEvent += unhandledInputBatch;
        return this;
    }

    public StateNodeBuilderSync<TStateKey, TEventKey> OnUnhandledInputKeyBatch(Action<IEnumerable<InputEvent>> unhandledInputKeyBatch) {
        OnUnhandledInputKeyBatchEvent += unhandledInputKeyBatch;
        return this;
    }
}