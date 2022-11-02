using System;
using System.Collections.Generic;

namespace Betauer.StateMachine {
    public abstract class BaseState<TStateKey, TEventKey> 
        where TStateKey : Enum where TEventKey : Enum {
        
        public TStateKey Key { get; }
        public Dictionary<TEventKey, Event<TStateKey, TEventKey>>? Events { get; }
        private readonly Condition<TStateKey, TEventKey>[]? _conditions;

        protected BaseState(
            TStateKey key,
            Dictionary<TEventKey, Event<TStateKey, TEventKey>>? events,
            Condition<TStateKey, TEventKey>[]? conditions) {
            Key = key;
            _conditions = conditions;
            Events = events;
        }

        public Command<TStateKey, TEventKey> Next(ConditionContext<TStateKey, TEventKey> ctx) {
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