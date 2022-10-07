using System;
using Godot;

namespace Betauer.StateMachine.Sync {
    public class StateNodeBuilderSync<TStateKey, TTransitionKey> : 
        BaseStateBuilderSync<StateNodeBuilderSync<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public StateNodeBuilderSync(TStateKey key, Action<IStateSync<TStateKey, TTransitionKey>> build) : base(key, build) {
        }

        protected override IStateSync<TStateKey, TTransitionKey> CreateState() {
            return new StateNodeSync<TStateKey, TTransitionKey>(Key, EnterFunc, ExecuteFunc, ExitFunc, SuspendFunc, AwakeFunc, Events, _input, _unhandledInput);
        }

        private Action<InputEvent> _input;
        private Action<InputEvent> _unhandledInput;
        
        public StateNodeBuilderSync<TStateKey, TTransitionKey> OnInput(Action<InputEvent> input) {
            _input = input;
            return this;
        }

        public StateNodeBuilderSync<TStateKey, TTransitionKey> OnUnhandledInput(Action<InputEvent> unhandledInput) {
            _unhandledInput = unhandledInput;
            return this;
        }
    }
}