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
        return new StateNodeSync<TStateKey, TEventKey>(Key, EnterFunc, Conditions.ToArray(), ExecuteFunc, ExitFunc, SuspendFunc, AwakeFunc, Events, _input, _unhandledInput, _inputBatch, _unhandledInputBatch);
    }

    private event Action<InputEvent>? _input;
    private event Action<InputEvent>? _unhandledInput;
    private event Action<IEnumerable<InputEvent>>? _inputBatch;
    private event Action<IEnumerable<InputEvent>>? _unhandledInputBatch;

    public StateNodeBuilderSync<TStateKey, TEventKey> OnInput(Action<InputEvent> input) {
        _input += input;
        return this;
    }
    public StateNodeBuilderSync<TStateKey, TEventKey> OnUnhandledInput(Action<InputEvent> unhandledInput) {
        _unhandledInput += unhandledInput;
        return this;
    }

    public StateNodeBuilderSync<TStateKey, TEventKey> OnInputBatch(Action<IEnumerable<InputEvent>> inputBatch) {
        _inputBatch += inputBatch;
        return this;
    }

    public StateNodeBuilderSync<TStateKey, TEventKey> OnUnhandledInputBatch(Action<IEnumerable<InputEvent>> unhandledInputBatch) {
        _unhandledInputBatch += unhandledInputBatch;
        return this;
    }
}