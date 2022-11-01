using System;

namespace Betauer.StateMachine {
    public abstract class BaseState<TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public TStateKey Key { get; }
        public EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>>>? Events { get; }
        private readonly Condition<TStateKey, TTransitionKey>[]? _conditions;

        protected BaseState(
            TStateKey key,
            EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>>>? events,
            Condition<TStateKey, TTransitionKey>[]? conditions) {
            Key = key;
            _conditions = conditions;
            Events = events;
        }

        public Command<TStateKey, TTransitionKey> Next(ConditionContext<TStateKey, TTransitionKey> ctx) {
            var span = _conditions.AsSpan();
            for (var i = 0; i < span.Length; i++) {
                var condition = span[i];
                if (condition.IsPredicateTrue()) {
                    return condition.GetResult(ctx);
                }
            }
            return ctx.None();
        }

    }
}