using System;
using Godot;

namespace Betauer.StateMachine {
    public class StateNodeBuilder<TStateKey, TTransitionKey> : 
        BaseStateBuilder<StateNodeBuilder<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public StateNodeBuilder(TStateKey key, Action<IState<TStateKey, TTransitionKey>> onBuild) : base(key, onBuild) {
        }

        protected override IState<TStateKey, TTransitionKey> CreateState() {
            return new StateNode<TStateKey, TTransitionKey>(_key, _enter, _execute, _exit, _suspend, _awake, _events, _input, _unhandledInput);
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