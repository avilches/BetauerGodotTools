using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.StateMachine.Sync {
    public class StateNodeSync<TStateKey, TTransitionKey> : StateSync<TStateKey, TTransitionKey> 
        where TTransitionKey : Enum where TStateKey : Enum {
        
        private readonly Action<InputEvent> _input;
        private readonly Action<InputEvent> _unhandledInput;

        public StateNodeSync(TStateKey key, 
            Action? enter,
            Tuple<Func<bool>, Func<ExecuteContext<TStateKey, TTransitionKey>, ExecuteContext<TStateKey, TTransitionKey>.Response>>[] conditions,
            Action execute,
            Action? exit, 
            Action? suspend, 
            Action? awake,
            EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>? events,
            Action<InputEvent> input, 
            Action<InputEvent> unhandledInput) : base(key, enter, conditions, execute, exit, suspend, awake, events) {
            _input = input;
            _unhandledInput = unhandledInput;
        }

        public void _Input(InputEvent e) => _input?.Invoke(e);
        public void _UnhandledInput(InputEvent e) => _unhandledInput?.Invoke(e);
    }
}