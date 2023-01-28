using System;
using Godot;

namespace Betauer.StateMachine.Sync;

public class StateNodeBuilderSync<TStateKey, TEventKey> :
    BaseStateBuilderSync<StateNodeBuilderSync<TStateKey, TEventKey>, TStateKey, TEventKey>
    where TStateKey : Enum where TEventKey : Enum {
    public StateNodeBuilderSync(TStateKey key, Action<IStateSync<TStateKey, TEventKey>> build) : base(key, build) {
    }

    protected override IStateSync<TStateKey, TEventKey> CreateState() {
        return new StateNodeSync<TStateKey, TEventKey>(
            Key,
            EventRules,
            Conditions?.ToArray(),
            EnterFunc,
            AwakeFunc,
            ExecuteFunc,
            SuspendFunc,
            ExitFunc,
            Input,
            ShortcutInput,
            UnhandledInput,
            UnhandledKeyInput);
    }

    private event Action<InputEvent>? Input;
    private event Action<InputEvent>? ShortcutInput;
    private event Action<InputEvent>? UnhandledInput;
    private event Action<InputEvent>? UnhandledKeyInput;

    public StateNodeBuilderSync<TStateKey, TEventKey> OnInput(Action<InputEvent> input) {
        Input += input;
        return this;
    }

    public StateNodeBuilderSync<TStateKey, TEventKey> OnShortcutInput(Action<InputEvent> shortcutInput) {
        ShortcutInput += shortcutInput;
        return this;
    }

    public StateNodeBuilderSync<TStateKey, TEventKey> OnUnhandledInput(Action<InputEvent> unhandledInput) {
        UnhandledInput += unhandledInput;
        return this;
    }

    public StateNodeBuilderSync<TStateKey, TEventKey> OnUnhandledKeyInput(Action<InputEvent> unhandledKeyInput) {
        UnhandledKeyInput += unhandledKeyInput;
        return this;
    }
}