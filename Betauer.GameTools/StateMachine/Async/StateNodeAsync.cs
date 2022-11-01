using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.StateMachine.Async {
    public class StateNodeAsync<TStateKey, TTransitionKey> : StateAsync<TStateKey, TTransitionKey> 
        where TTransitionKey : Enum where TStateKey : Enum {
        
        private readonly Action<InputEvent> _input;
        private readonly Action<InputEvent> _unhandledInput;
        
        public StateNodeAsync(TStateKey key,
            Func<Task>? enter,
            Tuple<Func<bool>, Func<ConditionContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>>>[] conditions,
            Func<Task> execute,
            Func<Task>? exit,
            Func<Task>? suspend,
            Func<Task>? awake,
            EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>>>? events,
            Action<InputEvent> input,
            Action<InputEvent> unhandledInput) : base(key, enter, conditions, execute, exit, suspend, awake, events) {
            _input = input;
            _unhandledInput = unhandledInput;
        }

        public void _Input(InputEvent e) => _input?.Invoke(e);
        public void _UnhandledInput(InputEvent e) => _unhandledInput?.Invoke(e);
    }
}