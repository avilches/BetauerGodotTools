using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.StateMachine.Async {
    public class StateNodeAsync<TStateKey, TTransitionKey> : StateAsync<TStateKey, TTransitionKey> 
        where TTransitionKey : Enum where TStateKey : Enum {
        
        private readonly Action<InputEvent> _input;
        private readonly Action<InputEvent> _unhandledInput;
        
        public StateNodeAsync(TStateKey key,
            Func<TStateKey, Task>? enter,
            Func<ExecuteContext<TStateKey, TTransitionKey>, Task<ExecuteContext<TStateKey, TTransitionKey>.Response>>? execute,
            Func<TStateKey, Task>? exit,
            Func<TStateKey, Task>? suspend,
            Func<TStateKey, Task>? awake,
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