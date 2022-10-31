using System;
using Godot;

namespace Betauer.StateMachine.Async {
    public class StateNodeBuilderAsync<TStateKey, TTransitionKey> : 
        BaseStateBuilderAsync<StateNodeBuilderAsync<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public StateNodeBuilderAsync(TStateKey key, Action<IStateAsync<TStateKey, TTransitionKey>> build) : base(key, build) {
        }

        protected override IStateAsync<TStateKey, TTransitionKey> CreateState() {
            return new StateNodeAsync<TStateKey, TTransitionKey>(Key, EnterFunc, Conditions.ToArray(), ExecuteFunc, ExitFunc, SuspendFunc, AwakeFunc, Events, _input, _unhandledInput);
        }

        private Action<InputEvent> _input;
        private Action<InputEvent> _unhandledInput;
        
        /// <summary>
        /// This event will be only executed when the state machine is available (not executing enter, exit, execute,
        /// suspend or awake). That means if one of these methods uses await for a long time, the input event will not
        /// be executed until they finish.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public StateNodeBuilderAsync<TStateKey, TTransitionKey> OnInput(Action<InputEvent> input) {
            _input = input;
            return this;
        }

        public StateNodeBuilderAsync<TStateKey, TTransitionKey> OnUnhandledInput(Action<InputEvent> unhandledInput) {
            _unhandledInput = unhandledInput;
            return this;
        }
    }
}