using System;

namespace Betauer.StateMachine.Sync {
    public class StateSync<TStateKey, TTransitionKey> : IStateSync<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        public TStateKey Key { get; }
        public EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>? Events { get; }

        private readonly Action<TStateKey>? _enter;
        private readonly Action<TStateKey>? _awake;
        private readonly Func<ExecuteContext<TStateKey, TTransitionKey>, ExecuteContext<TStateKey, TTransitionKey>.Response>? _execute;
        private readonly Action<TStateKey>? _suspend;
        private readonly Action<TStateKey>? _exit;

        public StateSync(
            TStateKey key,
            Action<TStateKey>? enter,
            Func<ExecuteContext<TStateKey, TTransitionKey>, ExecuteContext<TStateKey, TTransitionKey>.Response>? execute,
            Action<TStateKey>? exit,
            Action<TStateKey>? suspend,
            Action<TStateKey>? awake,
            EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>? events) {

            Key = key;
            _enter = enter;
            _execute = execute;
            _exit = exit;
            _suspend = suspend;
            _awake = awake;
            Events = events;
        }


        public void Enter(TStateKey from) {
            _enter?.Invoke(from);
        }

        public void Awake(TStateKey from) {
            _awake?.Invoke(from);
        }

        public ExecuteContext<TStateKey, TTransitionKey>.Response Execute(ExecuteContext<TStateKey, TTransitionKey> ctx) {
            return _execute != null ? _execute.Invoke(ctx) : ctx.None();
        }

        public void Suspend(TStateKey to) {
            _suspend?.Invoke(to);
        }

        public void Exit(TStateKey to) {
            _exit?.Invoke(to);
        }
    }
}