using System;
using Godot;

namespace Betauer.StateMachine.Sync {
    public class StateNodeBuilderSync<TStateKey, TEventKey> : 
        BaseStateBuilderSync<StateNodeBuilderSync<TStateKey, TEventKey>, TStateKey, TEventKey> 
        where TStateKey : Enum where TEventKey : Enum {
        
        public StateNodeBuilderSync(TStateKey key, Action<IStateSync<TStateKey, TEventKey>> build) : base(key, build) {
        }

        protected override IStateSync<TStateKey, TEventKey> CreateState() {
            return new StateNodeSync<TStateKey, TEventKey>(Key, EnterFunc, Conditions.ToArray(), ExecuteFunc, ExitFunc, SuspendFunc, AwakeFunc, Events, _input, _unhandledInput);
        }

        protected event Action<InputEvent>? _input;
        protected event Action<InputEvent>? _unhandledInput;
        
        public StateNodeBuilderSync<TStateKey, TEventKey> OnInput(Action<InputEvent> input) {
            _input += input;
            return this;
        }

        public StateNodeBuilderSync<TStateKey, TEventKey> OnUnhandledInput(Action<InputEvent> unhandledInput) {
            _unhandledInput += unhandledInput;
            return this;
        }
    }
}