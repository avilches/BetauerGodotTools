using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Betauer.StateMachine.Async {
    public class StateNodeAsync<TStateKey, TEventKey> : StateAsync<TStateKey, TEventKey> 
        where TEventKey : Enum where TStateKey : Enum {
        
        private readonly Action<InputEvent> _input;
        private readonly Action<InputEvent> _unhandledInput;
        
        public StateNodeAsync(TStateKey key,
            Func<Task>? enter,
            Condition<TStateKey, TEventKey>[] conditions,
            Func<Task> execute,
            Func<Task>? exit,
            Func<Task>? suspend,
            Func<Task>? awake,
            Dictionary<TEventKey, Event<TStateKey, TEventKey>>? events,
            Action<InputEvent> input,
            Action<InputEvent> unhandledInput) : base(key, enter, conditions, execute, exit, suspend, awake, events) {
            _input = input;
            _unhandledInput = unhandledInput;
        }

        public void _Input(InputEvent e) => _input?.Invoke(e);
        public void _UnhandledInput(InputEvent e) => _unhandledInput?.Invoke(e);
    }
}