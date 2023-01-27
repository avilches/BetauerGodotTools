using System;
using Godot;

namespace Betauer.StateMachine.Async;

public class StateNodeBuilderAsync<TStateKey, TEventKey> :
    BaseStateBuilderAsync<StateNodeBuilderAsync<TStateKey, TEventKey>, TStateKey, TEventKey>
    where TStateKey : Enum where TEventKey : Enum {
    public StateNodeBuilderAsync(TStateKey key, Action<IStateAsync<TStateKey, TEventKey>> build) : base(key, build) {
    }

    protected override IStateAsync<TStateKey, TEventKey> CreateState() {
        return new StateNodeAsync<TStateKey, TEventKey>(
            Key,
            EventRules,
            Conditions?.ToArray(),
            EnterFunc,
            ExecuteFunc,
            ExitFunc,
            SuspendFunc,
            AwakeFunc,
            Input,
            ShortcutInput,
            UnhandledInput,
            UnhandledKeyInput);
    }

    private event Action<InputEvent>? Input;
    private event Action<InputEvent>? ShortcutInput;
    private event Action<InputEvent>? UnhandledInput;
    private event Action<InputEvent>? UnhandledKeyInput;

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
}