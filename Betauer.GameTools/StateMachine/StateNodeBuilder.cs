using System;
using Godot;

namespace Betauer.StateMachine {
    public class StateNodeBuilder<TStateKey, TTransitionKey> : 
        BaseStateBuilder<StateNodeBuilder<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public StateNodeBuilder(TStateKey key, Action<IState<TStateKey, TTransitionKey>> build) : base(key, build) {
        }

        protected override IState<TStateKey, TTransitionKey> CreateState() {
            return new StateNode<TStateKey, TTransitionKey>(Key, EnterFunc, ExecuteFunc, ExitFunc, SuspendFunc, AwakeFunc, Events, _input, _unhandledInput);
        }

        private Action<InputEvent> _input;
        private Action<InputEvent> _unhandledInput;
        
        public StateNodeBuilder<TStateKey, TTransitionKey> OnInput(Action<InputEvent> input) {
            _input = input;
            return this;
        }

        public StateNodeBuilder<TStateKey, TTransitionKey> OnUnhandledInput(Action<InputEvent> unhandledInput) {
            _unhandledInput = unhandledInput;
            return this;
        }
    }
}