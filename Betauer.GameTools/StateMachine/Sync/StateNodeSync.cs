using System;
using Godot;

namespace Betauer.StateMachine.Sync {
    public class StateNodeSync<TStateKey, TTransitionKey> : StateSync<TStateKey, TTransitionKey> 
        where TTransitionKey : Enum where TStateKey : Enum {
        
        private readonly Action<InputEvent> _input;
        private readonly Action<InputEvent> _unhandledInput;

        public StateNodeSync(TStateKey key, 
            Action<TStateKey>? enter,
            Func<ExecuteContext<TStateKey, TTransitionKey>, ExecuteContext<TStateKey, TTransitionKey>.Response>? execute,
            Action<TStateKey>? exit, 
            Action<TStateKey>? suspend, 
            Action<TStateKey>? awake,
            EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>? events,
            Action<InputEvent> input, 
            Action<InputEvent> unhandledInput) : base(key, enter, execute, exit, suspend, awake, events) {
            _input = input;
            _unhandledInput = unhandledInput;
        }

        public void _Input(InputEvent e) => _input?.Invoke(e);
        public void _UnhandledInput(InputEvent e) => _unhandledInput?.Invoke(e);
    }
}