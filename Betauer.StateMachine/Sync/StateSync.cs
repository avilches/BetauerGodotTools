using System;

namespace Betauer.StateMachine.Sync {
    public class StateSync<TStateKey, TTransitionKey> : IStateSync<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public TStateKey Key { get; }
        public EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>? Events { get; }

        private readonly Action? _enter;
        private readonly Action? _awake;
        private readonly Tuple<Func<bool>, Func<Context<TStateKey, TTransitionKey>, Context<TStateKey, TTransitionKey>.Response>>[]? _conditions;
        private readonly Action? _execute;
        private readonly Action? _suspend;
        private readonly Action? _exit;

        public StateSync(
            TStateKey key,
            Action? enter,
            Tuple<Func<bool>, Func<Context<TStateKey, TTransitionKey>, Context<TStateKey, TTransitionKey>.Response>>[] conditions,
            Action? execute,
            Action? exit,
            Action? suspend,
            Action? awake,
            EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response>>? events) {

            Key = key;
            _enter = enter;
            _conditions = conditions;
            _execute = execute;
            _exit = exit;
            _suspend = suspend;
            _awake = awake;
            Events = events;
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

        public Context<TStateKey, TTransitionKey>.Response Next(Context<TStateKey, TTransitionKey> ctx) {
            var span = _conditions.AsSpan();
            for (var i = 0; i < span.Length; i++) {
                var condition = span[i];
                if (condition.Item1()) {
                    return condition.Item2(ctx);
                }
            }
            return ctx.None();
        }

        public void Suspend() {
            _suspend?.Invoke();
        }

        public void Exit() {
            _exit?.Invoke();
        }
    }
}