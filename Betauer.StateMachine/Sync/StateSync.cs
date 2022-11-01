using System;

namespace Betauer.StateMachine.Sync {
    public class StateSync<TStateKey, TTransitionKey> : BaseState<TStateKey, TTransitionKey>, IStateSync<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        
        private readonly Action? _enter;
        private readonly Action? _awake;
        private readonly Action? _execute;
        private readonly Action? _suspend;
        private readonly Action? _exit;

        public StateSync(
            TStateKey key,
            Action? enter,
            Condition<TStateKey, TTransitionKey>[] conditions,
            Action? execute,
            Action? exit,
            Action? suspend,
            Action? awake,
            EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>>>? events) : base(key, events, conditions ) {

            _enter = enter;
            _execute = execute;
            _exit = exit;
            _suspend = suspend;
            _awake = awake;
        }

        public void Enter() {
            _enter?.Invoke();
        }

        public void Awake() {
            _awake?.Invoke();
        }

        public void Execute() {
            _execute?.Invoke();
        }

        public void Suspend() {
            _suspend?.Invoke();
        }

        public void Exit() {
            _exit?.Invoke();
        }
    }
}